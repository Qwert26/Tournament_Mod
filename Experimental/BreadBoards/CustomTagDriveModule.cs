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
using System.Linq;
namespace Experimental.BreadBoards
{
    [BoardComponent("Custom Tag Driver","Input can be assigned to any custom tagged axis.")]
    public class CustomTagDriveModule : BreadboardModule
    {
        private float _input = 0f;
        public override Guid ComponentTypeId => new Guid("05D225F7-9338-4965-9DC9-87357A1F96A7");
        [Variable(0u,"Custom Tag")]
        public Var<string> CustomTag { get; set; } = new VarString("", 24);
        public override string Description => $"Sum of inputs is assigned to \"{CustomTag}\"-tagged axis.";
        public override string DescribeInput(int i)
        {
            return $"All inputs are added together and applied to the Tag {CustomTag}";
        }
        public override string DescribeOutput(int i)
        {
            return "No outputs";
        }
        public override string DescribeOutputLink(int index)
        {
            return "None";
        }
        public CustomTagDriveModule() {
            Width.Us = 150f;
            CreateInput();
        }
        public override int MaxInputs => 5;
        public override string GetString()
        {
            return $"{CustomTag}={Rounding.R2(_input)}";
        }
        public override bool Run()
        {
            _input = 0;
            if (AllInputsReadyOrNotLatched) {
                _input = BInputs.Us.Sum((BInput t) => t.GetCleanFloat());
                if (float.IsNaN(_input)) {
                    _input = 0;
                }
                if (!_construct.DockingRestricted.AmIBeingTractored()) {
                    _construct.ControlsRestricted.MakeRequestToCustomTag(CustomTag, _input);
                }
                return true;
            }
            return false;
        }
        public override void PopulateSegment(CircuitComponentEditorSegment segment, CircuitBoardDisplay displayer)
        {
            segment.Resize(1, 10);
            segment.AddInterpretter(TextInput<CustomTagDriveModule>.Quick(this, M.m((CustomTagDriveModule I) => I.CustomTag), "Custom Tag:", new ToolTip("Write the Custom Tag you want to send signals to."), delegate (CustomTagDriveModule I, string s)
            {
                I.CustomTag.Us = s;
            }));
        }
        public override void PopulateStandardSegment(CircuitComponentStandardEditorSegment segment, CircuitBoardDisplay displayer)
        {
            base.PopulateStandardSegment(segment, displayer);
            CreateAddInputButton(segment, displayer);
            CreateWidthAdjuster(segment);
        }
    }
}