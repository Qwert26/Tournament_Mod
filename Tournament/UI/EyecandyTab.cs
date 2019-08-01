using System;
using BrilliantSkies.Ui.Consoles;
using BrilliantSkies.Ui.Tips;
using BrilliantSkies.Ui.Consoles.Builders;
using BrilliantSkies.Ui.Consoles.Segments;
using BrilliantSkies.Ui.Consoles.Interpretters.Subjective.Numbers;
using BrilliantSkies.Ui.Consoles.Getters;
using BrilliantSkies.Ui.Consoles.Interpretters.Simple;
using UnityEngine;
using Tournament.Serialisation;
namespace Tournament.UI
{
    public class EyecandyTab : SuperScreen<Tournament>
    {
        private int currentTeam = 0;
        public EyecandyTab(ConsoleWindow window, Tournament focus) : base(window, focus)
        {
            Name = new Content("Eyecandy", "Change the fleet appearence of a Team.");
        }
        public override Action OnSelectTab => base.OnSelectTab;
        public override void Build()
        {
            base.Build();
            _focus.Parameters.EnsureEnoughData();
            ScreenSegmentStandard standard1 = CreateStandardSegment();
            standard1.AddInterpretter(new SubjectiveFloatClampedWithBar<TournamentParameters>(M.m<TournamentParameters>(0), M.m((TournamentParameters tp)=>tp.ActiveFactions-1),
                M.m((TournamentParameters tp)=>currentTeam), M.m<TournamentParameters>(1), _focus.Parameters, M.m<TournamentParameters>("Team Index"), delegate (TournamentParameters tp, float f)
                {
                    currentTeam = (int)f;
                    TriggerScreenRebuild();
                }, null, M.m<TournamentParameters>(new ToolTip("Which Team to edit"))));
            var header=CreateHeader(TournamentPlugin.factionManagement.factions[currentTeam].Name, new ToolTip("Edit Fleet colors for current Team"));
            ScreenSegmentTable table1 = CreateTableSegment(4, 6);
            table1.eTableOrder = ScreenSegmentTable.TableOrder.Columns;
            table1.SqueezeTable = false;
            ColorBuilder colorBuilder = new ColorBuilder(table1);

            table1.AddInterpretter(StringDisplay.Quick("Main Color"),0,0);
            colorBuilder.RedAdjust(_focus.Parameters.MainColorsPerTeam[currentTeam], M.m((Color c) => c.r), delegate (Color c, float f)
            {
                c.r = f;
            });
            colorBuilder.GreenAdjust(_focus.Parameters.MainColorsPerTeam[currentTeam], M.m((Color c) => c.g), delegate (Color c, float f)
            {
                c.g = f;
            });
            colorBuilder.BlueAdjust(_focus.Parameters.MainColorsPerTeam[currentTeam], M.m((Color c) => c.b), delegate (Color c, float f)
            {
                c.b = f;
            });
            colorBuilder.AlphaAdjust(_focus.Parameters.MainColorsPerTeam[currentTeam], M.m((Color c) => c.a), delegate (Color c, float f)
            {
                c.a = f;
            });
            colorBuilder.ShowColor(table1, _focus, M.m((Tournament t) => t.Parameters.MainColorsPerTeam[currentTeam]));

            table1.AddInterpretter(StringDisplay.Quick("Secondary Color"),0,1);
            colorBuilder.RedAdjust(_focus.Parameters.SecondaryColorsPerTeam[currentTeam], M.m((Color c)=>c.r), delegate (Color c, float f)
            {
                c.r = f;
            });
            colorBuilder.GreenAdjust(_focus.Parameters.SecondaryColorsPerTeam[currentTeam], M.m((Color c) => c.g), delegate (Color c, float f)
            {
                c.g = f;
            });
            colorBuilder.BlueAdjust(_focus.Parameters.SecondaryColorsPerTeam[currentTeam], M.m((Color c) => c.b), delegate (Color c, float f)
            {
                c.b = f;
            });
            colorBuilder.AlphaAdjust(_focus.Parameters.SecondaryColorsPerTeam[currentTeam], M.m((Color c) => c.a), delegate (Color c, float f)
            {
                c.a = f;
            });
            colorBuilder.ShowColor(table1, _focus, M.m((Tournament t) => t.Parameters.SecondaryColorsPerTeam[currentTeam]));

            table1.AddInterpretter(StringDisplay.Quick("Trim Color"),0,2);
            colorBuilder.RedAdjust(_focus.Parameters.TrimColorsPerTeam[currentTeam], M.m((Color c) => c.r), delegate (Color c, float f)
            {
                c.r = f;
            });
            colorBuilder.GreenAdjust(_focus.Parameters.TrimColorsPerTeam[currentTeam], M.m((Color c) => c.g), delegate (Color c, float f)
            {
                c.g = f;
            });
            colorBuilder.BlueAdjust(_focus.Parameters.TrimColorsPerTeam[currentTeam], M.m((Color c) => c.b), delegate (Color c, float f)
            {
                c.b = f;
            });
            colorBuilder.AlphaAdjust(_focus.Parameters.TrimColorsPerTeam[currentTeam], M.m((Color c) => c.a), delegate (Color c, float f)
            {
                c.a = f;
            });
            colorBuilder.ShowColor(table1, _focus, M.m((Tournament t) => t.Parameters.TrimColorsPerTeam[currentTeam]));

            table1.AddInterpretter(StringDisplay.Quick("Detail Color"),0,3);
            colorBuilder.RedAdjust(_focus.Parameters.DetailColorsPerTeam[currentTeam], M.m((Color c) => c.r), delegate (Color c, float f)
            {
                c.r = f;
            });
            colorBuilder.GreenAdjust(_focus.Parameters.DetailColorsPerTeam[currentTeam], M.m((Color c) => c.g), delegate (Color c, float f)
            {
                c.g = f;
            });
            colorBuilder.BlueAdjust(_focus.Parameters.DetailColorsPerTeam[currentTeam], M.m((Color c) => c.b), delegate (Color c, float f)
            {
                c.b = f;
            });
            colorBuilder.AlphaAdjust(_focus.Parameters.DetailColorsPerTeam[currentTeam], M.m((Color c) => c.a), delegate (Color c, float f)
            {
                c.a = f;
            });
            colorBuilder.ShowColor(table1, _focus, M.m((Tournament t) => t.Parameters.DetailColorsPerTeam[currentTeam]));

        }
    }
}
