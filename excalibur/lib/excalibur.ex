defmodule Excalibur do
  @moduledoc """
  Documentation for Excalibur.
  """
  
  @local_host {127, 0, 0, 1}

  def start() do
    IO.puts("p - publish, s - subscribe, u - unsubscribe, enter - receive")
    server = Socket.UDP.open! #create a socket with random port
    pid_self = self()
    spawn fn ->               #launch a process that will receive messages
      listen(server, pid_self)
    end
    user_IO(server)           #start the user interaction
  end

  def listen(server, pid_self) do
    {data, _client} = server |> Socket.Datagram.recv! #get data
    send(pid_self, {data})                            #send data to main process
#    IO.puts(data)
    listen(server, pid_self)                          #repeat
  end
  
  def user_IO(server) do
    receive do                                #receive messages
      {data} -> IO.puts("RECEIVED: #{data}")  #output the msg if received
    after                                     #
      100 -> IO.write("")                      #if nothing received, continue user interaction
    end
    
    {command, topic, message} = parse_input()
    
    cond do
      command == nil or topic == nil -> ""
      String.downcase(command) == "p" -> publish(server, topic, message)
      String.downcase(command) == "s" -> subscribe(server, topic)
      String.downcase(command) == "u" -> unsubscribe(server, topic)
      true -> IO.puts("Something's wrong")
    end
    user_IO(server)
  end
  
  def parse_input() do
    input = IO.gets("P|S|U topic message:")
    input = String.replace(input, "\n", "")
    input_parts = String.split(input, " ", trim: true, parts: 3)

    {command, input_parts} = List.pop_at(input_parts, 0)
    {topic, input_parts} = List.pop_at(input_parts, 0)
    {message, _} = List.pop_at(input_parts, 0)
    {command, topic, message}
  end
  
  def subscribe(server, topic) do
    Socket.Datagram.send!(server, "Subscribe,#{topic}", {@local_host, 10001})
  end

  def unsubscribe(server, topic) do
    Socket.Datagram.send!(server, "UnSubscribe,#{topic}", {@local_host, 10001})
  end

  def publish(server, topic, data) do
    Socket.Datagram.send!(server, "Publish,#{topic},#{data}", {@local_host, 10002})
  end
end

Excalibur.start()