defmodule Excalibur do
  @moduledoc """
  Documentation for Excalibur.
  """
  
  @local_host {127, 0, 0, 1}

  def main(_args \\ []) do
    start()
  end

  def start() do
    server = Socket.UDP.open!
    pid_self = self()
    spawn fn ->
      listen(server, pid_self)
    end
    
    
    IO.puts("begin")
    userIO(server)
  end

  def listen(server, pid_self) do  #, pid_self) do
    {data, _client} = server |> Socket.Datagram.recv!
    send(pid_self, {data})
#    IO.puts(data)
    listen(server, pid_self)
  end
  
  def userIO(server) do

    receive do
      {data} -> IO.puts(data)
    after
      1 -> IO.write("")
    end
    
    input = IO.gets("'P|S|U topic [message]:")
    input = String.replace(input, "\n", "")
    input_parts = String.split(input, " ", trim: true, parts: 3)
    {command, input_parts} = List.pop_at(input_parts, 0)
    {topic, input_parts} = List.pop_at(input_parts, 0)
    {message, _} = List.pop_at(input_parts, 0)
    
    cond do
      String.downcase(command) == "p" -> publish(server, topic, message)
      String.downcase(command) == "u" -> unsubscribe(server, topic)
      String.downcase(command) == "s" -> subscribe(server, topic)
      String.downcase(command) == "l" -> listen(server, self())
#        receive do
#          {data} -> IO.puts(data)
#        after
#          1000 -> IO.puts("")
#        end
      true -> IO.puts("Something wrong")
    end
    userIO(server)
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