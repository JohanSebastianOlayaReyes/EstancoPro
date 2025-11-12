const net = require('net');
const { spawn } = require('child_process');

const PORT = 4200;
const HOST = '127.0.0.1';

// Try connecting to the port. If we can connect, it's in use. If connection is refused, it's free.
const socket = new net.Socket();
let handled = false;

socket.setTimeout(1000);

socket.once('connect', () => {
  handled = true;
  console.error(`Port ${PORT} is already in use. Aborting to ensure a consistent dev port.`);
  socket.destroy();
  process.exit(1);
});

socket.once('timeout', () => {
  if (handled) return;
  handled = true;
  socket.destroy();
  // Port did not accept connection in time, assume it's free and start the server
  const child = spawn('npx', ['ng', 'serve', '--port', String(PORT)], { stdio: 'inherit', shell: true });
  child.on('exit', (code) => process.exit(code));
});

socket.once('error', (err) => {
  if (handled) return;
  // connection refused or unreachable implies port not in use
  handled = true;
  socket.destroy();
  const child = spawn('npx', ['ng', 'serve', '--port', String(PORT)], { stdio: 'inherit', shell: true });
  child.on('exit', (code) => process.exit(code));
});

socket.connect(PORT, HOST);
