using BrilliantSkies.Blocks;
using BrilliantSkies.Common.Circuits;
using BrilliantSkies.Common.Circuits.Ui;
using BrilliantSkies.Common.Circuits.Ui.Segments;
using BrilliantSkies.Core.Help;
using BrilliantSkies.Core.Serialisation.Parameters.Prototypes;
using BrilliantSkies.Core.Widgets;
using BrilliantSkies.Ui.Consoles.Getters;
using BrilliantSkies.Ui.Consoles.Interpretters.Subjective.Texts;
using BrilliantSkies.Ui.Tips;
using System;
namespace Experimental.BreadBoards
{
    [BoardComponent("Custom Tag Input", "Outputs the total propulsion request of the previous frame")]
    public class CustomTagInput : BreadboardModuleInput
    {
        public override Guid ComponentTypeId => new Guid("C94ED822-EF32-4DA5-B122-4438F781A78D");
        [Variable(0u,"Custom Tag")]
        public Var<string> CustomTag { get; set; } = new VarString("", 24);
        public override string Description => $"Outputs the last total prolusion request to with \"{CustomTag}\"-tagged axis.";
        public override string DescribeOutput(int i)
        {
            return $"Outputs the last total propulsion request to \"{CustomTag}\"-tagged axis.";
        }
        public override string DescribeOutputLink(int index)
        {
            return $"\"{CustomTag}\"={Rounding.R2(GetInput())}";
        }
        public override string GetString()
        {
            return $"\"{CustomTag}\"={Rounding.R2(GetInput())}";
        }
        public CustomTagInput() {
            Width.Us = 150;
            CreateOutput();
        }
        private float GetInput() {
            if (!_construct.ControlsRestricted.Last.GetCustomTags().TryGetValue(CustomTag, out float ret)) {
                ret = 0;
            }
            return ret;
        }
        public override bool Run()
        {
            AssignToOutputs(GetInput());
            return true;
        }
        public override void PopulateSegment(CircuitComponentEditorSegment segment, CircuitBoardDisplay displayer)
        {
            segment.Resize(1, 3);
            segment.AddInterpretter(TextInput<CustomTagInput>.Quick(this, M.m((CustomTagInput I) => I.CustomTag), "Custom Tag", new ToolTip("Write the Custom Tag you want to recieve signals from."), delegate (CustomTagInput I, string s)
            {
                I.CustomTag.Us = s;
            }));
        }
        public override void PopulateStandardSegment(CircuitComponentStandardEditorSegment segment, CircuitBoardDisplay displayer)
        {
            base.PopulateStandardSegment(segment, displayer);
            CreateWidthAdjuster(segment);
        }
    }
}