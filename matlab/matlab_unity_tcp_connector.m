% sendToUnity.m
% Simple example: connect to Unity, send a numeric value (as text), then close.
host = '127.0.0.1';
port = 6700;

try
    % Create TCP client and connect
    t = tcpclient(host, port, 'Timeout', 5);
    disp("Connected to Unity.");

    % Example: send a number (position x) as ASCII text followed by newline
    x = 200;  % change this value to move the Cylinder's x-position
    msg = sprintf('%.4f\n', x);
    write(t, uint8(msg));  % send bytes

    disp(['Sent message: ' msg]);
    % close
    clear t;
catch ME
    disp("Error sending to Unity:");
    disp(ME.message);
end
