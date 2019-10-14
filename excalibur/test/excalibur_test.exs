defmodule ExcaliburTest do
  use ExUnit.Case
  doctest Excalibur

  test "greets the world" do
    assert Excalibur.hello() == :world
  end
end
