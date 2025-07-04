import amqp from 'amqplib';
import { pool } from '../infra/db.js';
import dotenv from 'dotenv';
import CircuitBreaker from 'opossum';
import { setTimeout as sleep } from 'timers/promises';
dotenv.config();

const MAX_RETRIES = 5;

const saveToDatabase = async (payload) => {
  const user = payload.user;

  await pool.query(
    `INSERT INTO users (
      id, 
      name, 
      email, 
      registration_number, 
      user_avatar_url,
      user_type,
      operational_system,
      operational_system_version,
      device_model,
      device_manufacture_name
    )
    VALUES ($1, $2, $3, $4, $5, $6, $7, $8, $9, $10)
    ON CONFLICT (id) DO NOTHING`,
    [
      user.id,
      user.name,
      user.email,
      user.registration,
      user.photo ?? '',
      user.user_type,
      user.operational_system ?? '',
      user.operational_system_version ?? '',
      user.device_model ?? '',
      user.device_manufacture_name ?? ''
    ]
  );
};

const breaker = new CircuitBreaker(saveToDatabase, {
  timeout: 5000,
  errorThresholdPercentage: 50,
  resetTimeout: 10000,
});

breaker.fallback(() => {
  console.warn('Circuit breaker triggered. Saving message to DLQ.');
});

export async function startConsumer() {
  const connection = await amqp.connect(process.env.RABBITMQ_URL);
  const channel = await connection.createChannel();
  await channel.assertQueue(process.env.QUEUE_NAME, {
    durable: true
  });
  await channel.assertQueue(process.env.DLQ_NAME, { durable: true });

  channel.consume(
    process.env.QUEUE_NAME,
    async (msg) => {
      if (!msg) return;
      const content = msg.content.toString();
      console.log('Message received:', content);

      let payload;
      try {
        payload = JSON.parse(content);
      } catch (err) {
        console.error('Invalid JSON. Sending to DLQ.');
        await channel.sendToQueue(process.env.DLQ_NAME, Buffer.from(content), {
          persistent: true,
        });
        channel.ack(msg);
        return;
      }

      let retries = 0;
      let success = false;

      while (retries < MAX_RETRIES && !success) {
        try {
          await breaker.fire(payload);
          success = true;
          channel.ack(msg);
          console.log('Successfully saved to database');
        } catch (err) {
          retries++;
          console.warn(`Attempt ${retries}/${MAX_RETRIES} failed.`, err);
          await sleep(500 * retries);
        }
      }

      if (!success) {
        console.error('All attempts failed. Sending to DLQ.');
        await channel.sendToQueue(process.env.DLQ_NAME, Buffer.from(content), {
          persistent: true,
        });
        channel.ack(msg);
      }
    },
    { noAck: false }
  );
}
