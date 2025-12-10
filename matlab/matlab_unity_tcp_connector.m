host = '192.168.0.120';  % replace with Unity server's IP
port = 6700;

try
    t = tcpclient(host, port, 'Timeout', 5);
    disp("Connected to Unity.");

    % Send one newline-terminated message
    x = 200;
    msg = sprintf("%.4f\n", x);
    write(t, uint8(msg));

    disp(['Sent message: ' msg]);

    clear t;
catch ME
    disp("Error sending to Unity:");
    disp(ME.message);
end

