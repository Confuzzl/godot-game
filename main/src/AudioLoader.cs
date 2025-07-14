using Godot;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Matcha;
public partial class AudioLoader : Node
{
    private readonly FrozenDictionary<string, AudioStream[]> audio = new Dictionary<string, string[]>()
    {
        ["abc"] = []
    }.Select(kv => (Name: kv.Key, Audios: kv.Value.Select(name => new AudioStream()).ToArray()))
        .ToFrozenDictionary(kv => kv.Name, kv => kv.Audios);
    public override void _Ready()
    {

        //audio = new Dictionary<string, AudioStream[]>()
        //{
        //    ["abc"] = []
        //};
    }
}
