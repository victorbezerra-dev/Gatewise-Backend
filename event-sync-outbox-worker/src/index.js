import { pool } from './db.js';
import { publishToRabbitMQ } from './rabbitmq.js';

const WORKER_INTERVAL_MS = 5000;
const RETRY_ATTEMPTS = 10;
const RETRY_DELAY_MS = 2000;
const client = await pool.connect();

async function waitForDatabase() {
  await client.query(`
      CREATE TABLE IF NOT EXISTS outbox (
        id UUID PRIMARY KEY,
        event_type TEXT NOT NULL,
        payload JSONB NOT NULL,
        status TEXT NOT NULL,
        created_at TIMESTAMP NOT NULL DEFAULT now(),
        updated_at TIMESTAMP,
        attempts INT DEFAULT 0
      );
    `);
  for (let attempt = 1; attempt <= RETRY_ATTEMPTS; attempt++) {
    try {
      await pool.query('SELECT 1');
      console.log('Database is ready!');
      return;
    } catch (err) {
      console.warn(`⏳ Attempt ${attempt}/${RETRY_ATTEMPTS} failed: ${err.message}`);
      await new Promise(resolve => setTimeout(resolve, RETRY_DELAY_MS));
    }
  }
  throw new Error('Failed to connect to the database after multiple attempts.');
}

async function processOutboxEvents() {

  try {
    const { rows } = await client.query(`
      SELECT * FROM outbox
      WHERE status = 'PENDING'
      ORDER BY created_at ASC
      LIMIT 10
    `);

    for (const event of rows) {
      const { id, event_type, payload } = event;

      const success = await publishToRabbitMQ(event_type, payload);
      if (success) {
        await client.query(`
          UPDATE outbox
          SET status = 'SENT', updated_at = NOW()
          WHERE id = $1
        `, [id]);
        console.log(`Event ${id} sent successfully.`);
      } else {
        await client.query(`
          UPDATE outbox
          SET attempts = attempts + 1, updated_at = NOW()
          WHERE id = $1
        `, [id]);
        console.warn(`Failed to send event ${id}. Will retry later.`);
      }
    }

  } catch (err) {
    console.error("Worker error:", err.message);
  } finally {
    client.release();
  }
}

(async () => {
  await waitForDatabase();
  console.log(`Outbox worker started (running every ${WORKER_INTERVAL_MS / 1000}s)...`);
  setInterval(processOutboxEvents, WORKER_INTERVAL_MS);
})();
