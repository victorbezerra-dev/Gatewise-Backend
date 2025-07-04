import { startConsumer } from './worker/runner.js';

const MAX_RETRIES = 10;
const RETRY_DELAY_MS = 3000;

(async () => {
  let retries = 0;
  while (retries < MAX_RETRIES) {
    try {
      console.log(`Attempt ${retries + 1}/${MAX_RETRIES} to connect to RabbitMQ...`);
      await startConsumer();
      console.log('Consumer started successfully!');
      break;
    } catch (err) {
      retries++;
      console.error(`Connection failed: ${err.message}`);
      if (retries >= MAX_RETRIES) {
        console.error('Max retries reached. Exiting.');
        process.exit(1);
      }
      await new Promise((res) => setTimeout(res, RETRY_DELAY_MS));
    }
  }
})();
