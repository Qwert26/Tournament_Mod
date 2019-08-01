using System;
using BrilliantSkies.Ui.Consoles;
using BrilliantSkies.Ui.Tips;
using BrilliantSkies.Ui.Consoles.Segments;
using BrilliantSkies.Ui.Consoles.Interpretters.Subjective.Numbers;
using BrilliantSkies.Ui.Consoles.Getters;
using BrilliantSkies.Ui.Consoles.Interpretters.Subjective.Buttons;
using UnityEngine;
using Tournament.Serialisation;
using BrilliantSkies.Ui.Consoles.Interpretters.Subjective;
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
            for (int i = 0; i < _focus.Parameters.ActiveFactions; i++) {
                int index = i;
                CreateHeader("Team " + (1 + i), new ToolTip("Fleetcolors for Team " + (1 + i)));
                ScreenSegmentTable table = CreateTableSegment(4,1);
                table.SqueezeTable = false;
                table.SetConditionalDisplay(() => index < _focus.Parameters.ActiveFactions);
                table.AddInterpretter(new SubjectiveColorChanger<TournamentParameters>(_focus.Parameters, M.m<TournamentParameters>($"Team {i+1} Main Color"),
                    M.m<TournamentParameters>(new ToolTip($"Set the Main Color for Team {i+1}")), M.m((TournamentParameters tp) => tp.MainColorsPerTeam[index]), delegate (TournamentParameters tp, Color c)
                    {
                        tp.MainColorsPerTeam.Us[index] = c;
                    }));
                table.AddInterpretter(new SubjectiveColorChanger<TournamentParameters>(_focus.Parameters, M.m<TournamentParameters>($"Team {i+1} Secondary Color"),
                    M.m<TournamentParameters>(new ToolTip($"Set the Secondary Color for Team {i+1}")), M.m((TournamentParameters tp) => tp.SecondaryColorsPerTeam[index]), delegate (TournamentParameters tp, Color c)
                    {
                        tp.SecondaryColorsPerTeam.Us[index] = c;
                    }));
                table.AddInterpretter(new SubjectiveColorChanger<TournamentParameters>(_focus.Parameters, M.m<TournamentParameters>($"Team {i+1} Trim Color"),
                    M.m<TournamentParameters>(new ToolTip($"Set the Trim Color for Team {i+1}")), M.m((TournamentParameters tp) => tp.TrimColorsPerTeam[index]), delegate (TournamentParameters tp, Color c)
                    {
                        tp.TrimColorsPerTeam.Us[index] = c;
                    }));
                table.AddInterpretter(new SubjectiveColorChanger<TournamentParameters>(_focus.Parameters, M.m<TournamentParameters>($"Team {i+1} Detail Color"),
                    M.m<TournamentParameters>(new ToolTip($"Set the Main Color for Team {i+1}")), M.m((TournamentParameters tp) => tp.DetailColorsPerTeam[index]), delegate (TournamentParameters tp, Color c)
                    {
                        tp.DetailColorsPerTeam.Us[index] = c;
                    }));
            }
            CreateHeader("Prepared Fleet Color", new ToolTip("Here you can find prepared fleet colors from the old days."));
            CreateStandardSegment().AddInterpretter(SubjectiveFloatClampedWithBar<Tournament>.Quick(_focus, 0, _focus.Parameters.ActiveFactions - 1, 1, M.m((Tournament t) => currentTeam), "Current Team Index: {0}", delegate (Tournament t, float f)
            {
                currentTeam = (int)f;
            }, new ToolTip("Selects the Team to apply the prepared fleet colors to. Always one less than the team number")));
            foreach (TournamentFleetColor tfc in TournamentFleetColor.colorSchemes) {
                TournamentFleetColor current = tfc;
                ScreenSegmentStandard standard = CreateStandardSegment();
                standard.AddInterpretter(SubjectiveButton<TournamentParameters>.Quick(_focus.Parameters, current.Name, new ToolTip(current.Description), delegate (TournamentParameters tp)
                {
                    tp.MainColorsPerTeam.Us[currentTeam] = current.Main;
                    tp.SecondaryColorsPerTeam.Us[currentTeam] = current.Secondary;
                    tp.DetailColorsPerTeam.Us[currentTeam] = current.Detail;
                    tp.TrimColorsPerTeam.Us[currentTeam] = current.Trim;
                }));
                CreateSpace(5);
                ScreenSegmentTable table = CreateTableSegment(4, 1);
                table.SqueezeTable = false;
                table.AddInterpretter(new SubjectiveColorDisplay<TournamentFleetColor>(current, M.m<TournamentFleetColor>("Main Color"), M.m<TournamentFleetColor>(new ToolTip("")), M.m((TournamentFleetColor tFC) => tFC.Main)));
                table.AddInterpretter(new SubjectiveColorDisplay<TournamentFleetColor>(current, M.m<TournamentFleetColor>("Secondary Color"), M.m<TournamentFleetColor>(new ToolTip("")), M.m((TournamentFleetColor tFC) => tFC.Secondary)));
                table.AddInterpretter(new SubjectiveColorDisplay<TournamentFleetColor>(current, M.m<TournamentFleetColor>("Trim Color"), M.m<TournamentFleetColor>(new ToolTip("")), M.m((TournamentFleetColor tFC) => tFC.Trim)));
                table.AddInterpretter(new SubjectiveColorDisplay<TournamentFleetColor>(current, M.m<TournamentFleetColor>("Detail Color"), M.m<TournamentFleetColor>(new ToolTip("")), M.m((TournamentFleetColor tFC) => tFC.Detail)));
                CreateSpace(10);
            }
        }
    }
}
