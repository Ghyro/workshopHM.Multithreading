# workshopHW.Multithreading
## General
### Our task is to develop a client and server for a simple corporate chat. The main characteristics of our solution:
 - Server and client are implemented as usual console or GUI applications (of your choice)
 - The interaction between clients and the server is done via Named Pipes (System.IO.Pipes) or Sockets (System.Net.Sockets) - also for your choice. For ease of setup, you can store all connection parameters in code.
 - #### The client is a bot that, after starting, performs cyclically:
   - Connects with the new name to the server
   - Sends several messages to the server (messages are selected randomly from the prepared list, the number of sent messages and the pauses between them are also randomly set)
   - Receives all messages from the server that are displayed on the screen and / or saves to a file
   - Disconnects from server.
   - The cycle repeats until the user terminates the client or a server operation error occurs.
 - #### Server:
   - Accepts connection from client. When connected, it recognizes the name of the connected client.
   - Receives message strings from clients and sends them to other connected clients.
   - Stores the history of the last N messages that are sent to clients upon first connection.
   - At the end of the application sends a notification to clients and correctly closes all connections.

### Task 1
   - Implement the client and server using the “For each client - own processing flow” scheme for the server

   - Reading and writing can be done by synchronous operations.

### Task 2
   - Rewrite client and server using Task Parallel Library
