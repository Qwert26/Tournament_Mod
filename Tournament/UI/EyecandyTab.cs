using System;
using BrilliantSkies.Ui.Consoles;
using BrilliantSkies.Ui.Tips;
using BrilliantSkies.Ui.Consoles.Builders;
using BrilliantSkies.Ui.Consoles.Segments;
using BrilliantSkies.Ui.Consoles.Interpretters.Subjective.Buttons;
using BrilliantSkies.Ui.Consoles.Getters;
namespace Tournament.UI
{
    public class EyecandyTab : SuperScreen<Tournament>
    {
        public EyecandyTab(ConsoleWindow window, Tournament focus) : base(window, focus)
        {
            Name = new Content("Eyecandy", "The two generated Teams came with their own fleet colors. You can change them here.");
        }
        public override Action OnSelectTab => base.OnSelectTab;
        public override void Build()
        {
            base.Build();
            CreateHeader("Team 1 Fleet Colors",new ToolTip(""));
            ScreenSegmentTable table1 = CreateTableSegment(4, 6);
            table1.SetColumnHeadings("Main", "Secondary", "Trim", "Detail");
            table1.eTableOrder = ScreenSegmentTable.TableOrder.Columns;
            ColorBuilder cb = new ColorBuilder(table1);
            cb.RgbAdjust(_focus.Parameters.Team1Main, true);
            cb.ShowColor(table1, _focus.Parameters, M.m<TournamentParameters>(_focus.Parameters.Team1Main));
            table1.AddInterpretter(SubjectiveButton<TournamentParameters>.Quick(_focus.Parameters, "Reset", new ToolTip("Reset the main fleet color of Team 1."), (tp) => tp.Team1Main.Reset()));
            cb.RgbAdjust(_focus.Parameters.Team1Secondary, true);
            cb.ShowColor(table1, _focus.Parameters, M.m<TournamentParameters>(_focus.Parameters.Team1Secondary));
            table1.AddInterpretter(SubjectiveButton<TournamentParameters>.Quick(_focus.Parameters, "Reset", new ToolTip("Reset the secondary fleet color of Team 1."), (tp) => tp.Team1Secondary.Reset()));
            cb.RgbAdjust(_focus.Parameters.Team1Trim, true);
            cb.ShowColor(table1, _focus.Parameters, M.m<TournamentParameters>(_focus.Parameters.Team1Trim));
            table1.AddInterpretter(SubjectiveButton<TournamentParameters>.Quick(_focus.Parameters, "Reset", new ToolTip("Reset the trim fleet color of Team 1."), (tp) => tp.Team1Trim.Reset()));
            cb.RgbAdjust(_focus.Parameters.Team1Detail, true);
            cb.ShowColor(table1, _focus.Parameters, M.m<TournamentParameters>(_focus.Parameters.Team1Detail));
            table1.AddInterpretter(SubjectiveButton<TournamentParameters>.Quick(_focus.Parameters, "Reset", new ToolTip("Reset the detail fleet color of Team 1."), (tp) => tp.Team1Detail.Reset()));

            CreateHeader("Team 2 Fleet Colors", new ToolTip(""));
            ScreenSegmentTable table2 = CreateTableSegment(4, 6);
            table2.SetColumnHeadings("Main", "Secondary", "Trim", "Detail");
            table2.eTableOrder = ScreenSegmentTable.TableOrder.Columns;
            cb = new ColorBuilder(table2);
            cb.RgbAdjust(_focus.Parameters.Team2Main, true);
            cb.ShowColor(table2, _focus.Parameters, M.m<TournamentParameters>(_focus.Parameters.Team2Main));
            table2.AddInterpretter(SubjectiveButton<TournamentParameters>.Quick(_focus.Parameters, "Reset", new ToolTip("Reset the main fleet color of Team 2."), (tp) => tp.Team2Main.Reset()));
            cb.RgbAdjust(_focus.Parameters.Team2Secondary, true);
            cb.ShowColor(table2, _focus.Parameters, M.m<TournamentParameters>(_focus.Parameters.Team2Secondary));
            table2.AddInterpretter(SubjectiveButton<TournamentParameters>.Quick(_focus.Parameters, "Reset", new ToolTip("Reset the secondary fleet color of Team 2."), (tp) => tp.Team2Secondary.Reset()));
            cb.RgbAdjust(_focus.Parameters.Team2Trim, true);
            cb.ShowColor(table2, _focus.Parameters, M.m<TournamentParameters>(_focus.Parameters.Team2Trim));
            table2.AddInterpretter(SubjectiveButton<TournamentParameters>.Quick(_focus.Parameters, "Reset", new ToolTip("Reset the trim fleet color of Team 2."), (tp) => tp.Team2Trim.Reset()));
            cb.RgbAdjust(_focus.Parameters.Team2Detail, true);
            cb.ShowColor(table2, _focus.Parameters, M.m<TournamentParameters>(_focus.Parameters.Team2Detail));
            table2.AddInterpretter(SubjectiveButton<TournamentParameters>.Quick(_focus.Parameters, "Reset", new ToolTip("Reset the detail fleet color of Team 2."), (tp) => tp.Team2Detail.Reset()));
        }
    }
}
