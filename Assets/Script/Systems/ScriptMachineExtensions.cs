using Unity.VisualScripting;

public static class ScriptMachineExtensions
{
    public static void SetGraph(this ScriptMachine machine, ScriptGraphAsset newGraph)
    {
        if (machine == null || newGraph == null)
            return;

        machine.nest.source = GraphSource.Macro;
        machine.nest.macro = newGraph;
    }
}
