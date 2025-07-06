const amqp = require('amqplib');

const RABBITMQ_URL = 'amqp://guest:guest@localhost:5672';
const QUEUE_NAME = 'user.login.sync';

async function publishToRabbitMQ(eventType, message) {
  try {
    const connection = await amqp.connect(RABBITMQ_URL);
    const channel = await connection.createChannel();

    await channel.assertQueue(QUEUE_NAME, { durable: true });
    channel.sendToQueue(QUEUE_NAME, Buffer.from(
      typeof message === 'string' ? message : JSON.stringify(message)
    ), {
      persistent: true
    });

    await channel.close();
    await connection.close();

    return true;
  } catch (err) {
    console.error("Failed to publish to RabbitMQ:", err.message);
    return false;
  }
}

module.exports = { publishToRabbitMQ };
