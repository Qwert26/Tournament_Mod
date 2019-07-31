using System;
using BrilliantSkies.Ui.Consoles;
using BrilliantSkies.Ui.Tips;
using BrilliantSkies.Ui.Consoles.Builders;
using BrilliantSkies.Ui.Consoles.Segments;
using BrilliantSkies.Ui.Consoles.Interpretters.Subjective.Numbers;
using BrilliantSkies.Ui.Consoles.Getters;
using BrilliantSkies.Ui.Consoles.Interpretters.Simple;
using UnityEngine;
namespace Tournament.UI
{
    public class EyecandyTab : SuperScreen<Tournament>
    {
        private int currentTeam = 0;
        public EyecandyTab(ConsoleWindow window, Tournament focus) : base(window, focus)
        {
            Name = new Content("Eyecandy", "");
        }
        public override Action OnSelectTab => base.OnSelectTab;
        public override void Build()
        {
            base.Build();
            _focus.Parameters.EnsureEnoughData();
            ScreenSegmentStandard standard1 = CreateStandardSegment();
            standard1.AddInterpretter(new SubjectiveFloatClamped<Tournament>(M.m<Tournament>(0), M.m<Tournament>(_focus.Parameters.ActiveFactions), M.m<Tournament>(currentTeam),
                M.m<Tournament>(1), _focus, M.m<Tournament>("Team Index"), delegate (Tournament t, float f)
                {
                    currentTeam = (int)f;
                }, null, M.m<Tournament>(new ToolTip(""))));
            CreateHeader(TournamentPlugin.factionManagement.factions[currentTeam].Name, new ToolTip("Edit Fleet colors for current Team"));
            ScreenSegmentTable table1 = CreateTableSegment(4, 6);
            table1.eTableOrder = ScreenSegmentTable.TableOrder.Columns;
            ColorBuilder colorBuilder = new ColorBuilder(table1);

            table1.AddInterpretter(StringDisplay.Quick("Main Color"));
            colorBuilder.RedAdjust(_focus.Parameters.MainColorsPerTeam[currentTeam], M.m<Color>(_focus.Parameters.MainColorsPerTeam[currentTeam].r), delegate (Color c, float f)
            {
                c.r = f;
            });
            colorBuilder.GreenAdjust(_focus.Parameters.MainColorsPerTeam[currentTeam], M.m<Color>(_focus.Parameters.MainColorsPerTeam[currentTeam].g), delegate (Color c, float f)
            {
                c.g = f;
            });
            colorBuilder.BlueAdjust(_focus.Parameters.MainColorsPerTeam[currentTeam], M.m<Color>(_focus.Parameters.MainColorsPerTeam[currentTeam].b), delegate (Color c, float f)
            {
                c.b = f;
            });
            colorBuilder.AlphaAdjust(_focus.Parameters.MainColorsPerTeam[currentTeam], M.m<Color>(_focus.Parameters.MainColorsPerTeam[currentTeam].a), delegate (Color c, float f)
            {
                c.a = f;
            });
            colorBuilder.ShowColor(table1, _focus, M.m<Tournament>(_focus.Parameters.MainColorsPerTeam[currentTeam]));

            table1.AddInterpretter(StringDisplay.Quick("Secondary Color"));
            colorBuilder.RedAdjust(_focus.Parameters.SecondaryColorsPerTeam[currentTeam], M.m<Color>(_focus.Parameters.SecondaryColorsPerTeam[currentTeam].r), delegate (Color c, float f)
            {
                c.r = f;
            });
            colorBuilder.GreenAdjust(_focus.Parameters.SecondaryColorsPerTeam[currentTeam], M.m<Color>(_focus.Parameters.SecondaryColorsPerTeam[currentTeam].g), delegate (Color c, float f)
            {
                c.g = f;
            });
            colorBuilder.BlueAdjust(_focus.Parameters.SecondaryColorsPerTeam[currentTeam], M.m<Color>(_focus.Parameters.SecondaryColorsPerTeam[currentTeam].b), delegate (Color c, float f)
            {
                c.b = f;
            });
            colorBuilder.AlphaAdjust(_focus.Parameters.SecondaryColorsPerTeam[currentTeam], M.m<Color>(_focus.Parameters.SecondaryColorsPerTeam[currentTeam].a), delegate (Color c, float f)
            {
                c.a = f;
            });
            colorBuilder.ShowColor(table1, _focus, M.m<Tournament>(_focus.Parameters.SecondaryColorsPerTeam[currentTeam]));

            table1.AddInterpretter(StringDisplay.Quick("Trim Color"));
            colorBuilder.RedAdjust(_focus.Parameters.TrimColorsPerTeam[currentTeam], M.m<Color>(_focus.Parameters.TrimColorsPerTeam[currentTeam].r), delegate (Color c, float f)
            {
                c.r = f;
            });
            colorBuilder.GreenAdjust(_focus.Parameters.TrimColorsPerTeam[currentTeam], M.m<Color>(_focus.Parameters.TrimColorsPerTeam[currentTeam].g), delegate (Color c, float f)
            {
                c.g = f;
            });
            colorBuilder.BlueAdjust(_focus.Parameters.TrimColorsPerTeam[currentTeam], M.m<Color>(_focus.Parameters.TrimColorsPerTeam[currentTeam].b), delegate (Color c, float f)
            {
                c.b = f;
            });
            colorBuilder.AlphaAdjust(_focus.Parameters.TrimColorsPerTeam[currentTeam], M.m<Color>(_focus.Parameters.TrimColorsPerTeam[currentTeam].a), delegate (Color c, float f)
            {
                c.a = f;
            });
            colorBuilder.ShowColor(table1, _focus, M.m<Tournament>(_focus.Parameters.TrimColorsPerTeam[currentTeam]));

            table1.AddInterpretter(StringDisplay.Quick("Detail Color"));
            colorBuilder.RedAdjust(_focus.Parameters.DetailColorsPerTeam[currentTeam], M.m<Color>(_focus.Parameters.DetailColorsPerTeam[currentTeam].r), delegate (Color c, float f)
            {
                c.r = f;
            });
            colorBuilder.GreenAdjust(_focus.Parameters.DetailColorsPerTeam[currentTeam], M.m<Color>(_focus.Parameters.DetailColorsPerTeam[currentTeam].g), delegate (Color c, float f)
            {
                c.g = f;
            });
            colorBuilder.BlueAdjust(_focus.Parameters.DetailColorsPerTeam[currentTeam], M.m<Color>(_focus.Parameters.DetailColorsPerTeam[currentTeam].b), delegate (Color c, float f)
            {
                c.b = f;
            });
            colorBuilder.AlphaAdjust(_focus.Parameters.DetailColorsPerTeam[currentTeam], M.m<Color>(_focus.Parameters.DetailColorsPerTeam[currentTeam].a), delegate (Color c, float f)
            {
                c.a = f;
            });
            colorBuilder.ShowColor(table1, _focus, M.m<Tournament>(_focus.Parameters.DetailColorsPerTeam[currentTeam]));
        }
    }
}
