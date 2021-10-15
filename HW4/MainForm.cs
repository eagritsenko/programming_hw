using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Reflection;
using System.Xml;

namespace HW4
{
    public partial class MainForm : Form
    {
        private static Random r = new Random();
        /// <summary>
        /// Path to autosave games at.
        /// </summary>
        private static string autosavePath = "autosave.xml";
        /// <summary>
        /// Array of strings corresponding to possible game results;
        /// </summary>
        private static string[] gameResultStrings = new string[] { "Victory!", "Victory! (Not yours.)", "Draw" };
        /// <summary>
        /// This form's size before resizing.
        /// </summary>
        Size oldSize;
        /// <summary>
        /// Indicates whether this form is at game state.s
        /// </summary>
        bool atGameState;
        /// <summary>
        /// Index of health field collumn.
        /// </summary>
        int healthCollumnIndex = 0;
        /// <summary>
        /// Path to CSV unit files
        /// </summary>
        string pathToUnitsCSV = "dota2.CSV";
        /// <summary>
        /// Sorted dictionary of units loaded.
        /// </summary>
        SortedDictionary<string, Unit> units;
        /// <summary>
        /// List of units currently visible.
        /// </summary>
        List<Unit> visibleUnits = new List<Unit>();
        GameEnvironment game;

        /// <summary>
        /// Sets status bar to display error status text provided.
        /// </summary>
        private void SetErrorStatus(string s)
        {
            tsslStatus.ForeColor = Color.DarkRed;
            tsslStatus.Text = s;
        }

        /// <summary>
        /// Sets status bar to display info status text provided.
        /// </summary>
        private void SetInfoStatus(string s)
        {
            tsslStatus.ForeColor = SystemColors.InfoText;
            tsslStatus.Text = s;
        }

        /// <summary>
        /// Append Unit row to the table and performs relayted operations.
        /// </summary>
        private void AppendRow(Unit unit)
        {
            DataGridViewRow row = new DataGridViewRow();
            row.CreateCells(dgvUnitTable);
            for(int i = 0; i < Unit.DisplayedProperties.Length; i++)
            {
                ValueDisplayAttribute cAttribute =
                    (ValueDisplayAttribute)Attribute.GetCustomAttribute(Unit.DisplayedProperties[i], typeof(ValueDisplayAttribute));
                row.Cells[i].ReadOnly = cAttribute.ReadOnly;
                row.Cells[i].Value = Unit.DisplayedProperties[i].GetValue(unit);
            }
            row.Tag = unit;
            row.HeaderCell.Value = "Unit";
            dgvUnitTable.Rows.Add(row);
            visibleUnits.Add(unit);
        }

        /// <summary>
        /// Append PlayingUnit row to the table and performs relayted operations.
        /// </summary>
        private void AppendPlayingUnitRow(PlayingUnit unit, string playerName)
        {
            DataGridViewRow row = new DataGridViewRow();
            row.CreateCells(dgvUnitTable);
            row.HeaderCell.Value = playerName;
            for (int i = 0; i < Unit.DisplayedProperties.Length; i++)
            {
                row.Cells[i].ReadOnly = true;
                row.Cells[i].Value = Unit.DisplayedProperties[i].GetValue(unit);
            }
            row.Tag = unit;
            dgvUnitTable.Rows.Add(row);
        }

        /// <summary>
        /// Checks whether the unit passes all filters in the filter row.
        /// </summary>
        private bool PassesFilter(Unit unit)
        {
            for(int i = 0; i < Unit.DisplayedProperties.Length; i++)
            {
                var filter = dgvUnitTable.Rows[0].Cells[i].Value;
                if (filter == null)
                    continue;
                MethodInfo filterMethod = (MethodInfo)dgvUnitTable.Rows[0].Cells[i].Tag;
                var value = Unit.DisplayedProperties[i].GetValue(unit);
                if (!(bool)filterMethod.Invoke(filter, new object[]{ value}))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Filters unit table using filters in the first row.
        /// </summary>
        private void FilterUnitTable()
        {
            dgvUnitTable.Enabled = false;
            SetInfoStatus("Filtering...");
            visibleUnits.Clear();
            foreach (DataGridViewRow row in dgvUnitTable.Rows)
            {
                if(row.Tag is Unit)
                {
                    if (PassesFilter((Unit)row.Tag))
                    {
                        row.Visible = true;
                        visibleUnits.Add((Unit)row.Tag);
                    }
                    else
                        row.Visible = false;
                }
            }
            dgvUnitTable.Enabled = true;
            SetInfoStatus("OK");
        }

        /// <summary>
        /// Loads units table from the CSV file.
        /// </summary>
        private void LoadUnitsTable()
        {
            units = new SortedDictionary<string, Unit>();
            SetInfoStatus("Loading units table...");
            if (!File.Exists(pathToUnitsCSV))
                SetErrorStatus("Loading error. CSV unit table file does not exist." +
                    $"It should be placed in the same directory as the program and named {pathToUnitsCSV}");

            FileStream file;
            try
            {
                file = File.OpenRead(pathToUnitsCSV);
            }
            catch
            {
                SetErrorStatus("Loading error. Unable to open CSV unit table file.");
                return;
            }

            CSVReader csvReader = new CSVReader(file, Encoding.UTF8, ';');
            UnitEntryReader reader;
            try
            {
                reader = new UnitEntryReader(csvReader);
                while (!reader.EOF)
                {
                    var current = reader.ReadEntry();
                    var cUnit = new Unit(current);
                    units.Add(current.name, cUnit);
                    visibleUnits.Add(cUnit);
                    AppendRow(cUnit);
                }
            }
            catch(Exception ex)
            {
                SetErrorStatus($"Loading error. Reading error: {ex.Message}");
                return;
            }
            csvReader.Dispose();
            SetInfoStatus("Table loaded.");
        }

        /// <summary>
        /// Initializes data grid view collumns
        /// </summary>
        public void InitializeDGVCollumns()
        {
            foreach(var property in Unit.DisplayedProperties)
            {
                ValueDisplayAttribute cAttribute = 
                    (ValueDisplayAttribute)Attribute.GetCustomAttribute(property, typeof(ValueDisplayAttribute));
                DataGridViewTextBoxCell template = new DataGridViewTextBoxCell();
                DataGridViewColumn dgvCollumn = new DataGridViewColumn(template);
                dgvCollumn.Name = property.Name;
                dgvCollumn.DataPropertyName = property.Name;
                dgvCollumn.ValueType = property.PropertyType;
                dgvCollumn.HeaderText = cAttribute.HeaderText ?? property.Name;
                // dgvCollumn.ReadOnly = cAttribute.ReadOnly;
                dgvUnitTable.Columns.Add(dgvCollumn);
            }
        }
        
        /// <summary>
        /// Initializes row of value filters.
        /// </summary>
        private void InitializeValueFilterRow()
        {
            DataGridViewRow row = new DataGridViewRow();
            row.CreateCells(dgvUnitTable);
            row.HeaderCell.Value = "Filter";
            row.Frozen = true;
            for(int i = 0; i < Unit.DisplayedProperties.Length; i++)
            {
                Type cValueFilter = typeof(ValueFilter<>).MakeGenericType(Unit.DisplayedProperties[i].PropertyType);
                row.Cells[i].ReadOnly = false;
                row.Cells[i].ValueType = cValueFilter;
                row.Cells[i].Value = Activator.CreateInstance(cValueFilter);
                row.Cells[i].Style.BackColor = SystemColors.Control;
                row.Cells[i].Tag = cValueFilter.GetMethod(nameof(ValueFilter<string>.Satisfies));
            }
            row.Tag = typeof(ValueFilter<>);
            dgvUnitTable.Rows.Add(row);
        }

        public MainForm()
        {
            InitializeComponent();
            // Designer crushes sometimes if this is set in designer
            attackToolStripMenuItem.Tag = PlayingUnit.UnitState.Attacking;
            defenceToolStripMenuItem.Tag = PlayingUnit.UnitState.Defence;
            runToolStripMenuItem.Tag = PlayingUnit.UnitState.Running;
        }

        /// <summary>
        /// Actions to perform on form load.
        /// </summary>
        private void Form1_Load(object sender, EventArgs e)
        {
            InitializeDGVCollumns();
            InitializeValueFilterRow();
            LoadUnitsTable();
            oldSize = Size;
        }

        /// <summary>
        /// Validates a unit row cell using indecies from DataGridViewCellEventArgs provided.
        /// </summary>
        private void ValidateUnitCell(DataGridViewCellEventArgs e)
        {
            var unit = ((Unit)dgvUnitTable.Rows[e.RowIndex].Tag);
            var newValue = dgvUnitTable.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
            try
            {
                // It appeared to be costly to separate validation from the property
                Unit.DisplayedProperties[e.ColumnIndex].SetValue(unit, newValue);
                SetInfoStatus("OK");
            }
            catch (System.Reflection.TargetInvocationException ex)
            {
                if (!(ex.InnerException is ArgumentException))
                    throw ex;
                string headerText = dgvUnitTable.Columns[e.ColumnIndex].HeaderText;
                string unitName = unit.Name;
                var propertyValue = Unit.DisplayedProperties[e.ColumnIndex].GetValue(unit);
                dgvUnitTable.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = propertyValue;
                SetErrorStatus($"Unable to set {headerText} for {unitName}: {ex.InnerException.Message}.");
            }
            catch
            {
                string headerText = dgvUnitTable.Columns[e.ColumnIndex].HeaderText;
                string unitName = unit.Name;
                var propertyValue = Unit.DisplayedProperties[e.ColumnIndex].GetValue(unit);
                dgvUnitTable.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = propertyValue;
                SetErrorStatus($"Unable to set given {headerText} value for {unitName}.");
            }
        }

        /// <summary>
        /// Actions to perform on data grid view data error.
        /// </summary>
        private void DgvUnitTable_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (dgvUnitTable.Rows[e.RowIndex].Tag is Unit)
            {
                Unit unit = ((Unit)dgvUnitTable.Rows[e.RowIndex].Tag);
                string headerText = dgvUnitTable.Columns[e.ColumnIndex].HeaderText;
                string unitName = unit.Name;
                SetErrorStatus($"Unable to set {headerText} for {unitName}: Incorrect fromat.");
            }
            else
            {
                string headerText = dgvUnitTable.Columns[e.ColumnIndex].HeaderText;
                SetErrorStatus($"Unable to set {headerText} for row {e.RowIndex}: Incorrect fromat.");
            }
        }

        /// <summary>
        /// Actions to perform if cell value has changed.
        /// </summary>
        private void DgvUnitTable_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (!atGameState)
            {
                if (dgvUnitTable.Rows[e.RowIndex].Tag is Unit)
                {
                    ValidateUnitCell(e);
                    dgvUnitTable.Rows[e.RowIndex].Visible = PassesFilter((Unit)dgvUnitTable.Rows[e.RowIndex].Tag);
                }
                else
                    FilterUnitTable();
            }
        }

        /// <summary>
        /// Cleans up elmenent specific to unit table view.
        /// </summary>
        private void DisposeUnitTableView()
        {
            visibleUnits.Clear();
            dgvUnitTable.Rows.Clear();
            playGameToolStripMenuItem.Visible = false;
        }

        /// <summary>
        /// Cleans up elements specific to game table view.
        /// </summary>
        private void DisposeGameTableView()
        {
            atGameState = false;
            dgvUnitTable.Rows.Clear();
            dgvUnitTable.Columns[nameof(Unit.Health)].DisplayIndex = dgvUnitTable.Columns[nameof(Unit.Health)].Index;
            dgvUnitTable.Columns[nameof(Unit.MaxHealth)].DisplayIndex = dgvUnitTable.Columns[nameof(Unit.MaxHealth)].Index;
            attackToolStripMenuItem.Visible = false;
            runToolStripMenuItem.Visible = false;
            defenceToolStripMenuItem.Visible = false;
            saveGameToolStripMenuItem.Visible = false;
        }

        /// <summary>
        /// Initializes appearance of certain elements specific to game table view.
        /// </summary>
        private void InitializeGameTableViewAppearance()
        {
            dgvUnitTable.Columns[nameof(Unit.Health)].DisplayIndex = 1;
            dgvUnitTable.Columns[nameof(Unit.MaxHealth)].DisplayIndex = 2;
            healthCollumnIndex = dgvUnitTable.Columns[nameof(Unit.Health)].Index;
            attackToolStripMenuItem.Visible = true;
            runToolStripMenuItem.Visible = true;
            defenceToolStripMenuItem.Visible = true;
            saveGameToolStripMenuItem.Visible = true;
        }

        /// <summary>
        /// Actions to perform if "Play game" tool strip button is clicked.
        /// </summary>
        private void PlayGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dgvUnitTable.SelectedRows.Count == 0 || !(dgvUnitTable.SelectedRows[0].Tag is Unit))
                SetErrorStatus("Unable to start the game: you have to select a unit row.");
            else if (visibleUnits.Count < 2)
                SetErrorStatus("Unable to start the game: at least two units should be passing filter.");
            else
            {
                Unit selected = (Unit)dgvUnitTable.SelectedRows[0].Tag;
                Unit enemy;
                visibleUnits.Remove(selected);
                enemy = visibleUnits[r.Next(visibleUnits.Count)];
                DisposeUnitTableView();
                game = new GameEnvironment(selected, enemy);
                game.OnGameFinished += OnGameFinished;
                AppendPlayingUnitRow(game.First, "You");
                AppendPlayingUnitRow(game.Second, "Enemy");
                InitializeGameTableViewAppearance();
                atGameState = true;
            }

        }

        /// <summary>
        /// Actions to perform on game finished.
        /// </summary>
        /// <param name="game"></param>
        private void OnGameFinished(GameEnvironment game)
        {
            string result = gameResultStrings[(int)game.Result - 1];
            try
            {
                if (File.Exists(autosavePath))
                    File.Delete(autosavePath);
            }
            catch { }
            SetInfoStatus(result);
            MessageBox.Show(result);
        }

        /// <summary>
        /// Sets random state to the PlayingUnit given
        /// </summary>
        private void SetRandomState(PlayingUnit u)
        {
            u.State = PlayingUnit.States[r.Next(0, PlayingUnit.States.Length)];
        }

        /// <summary>
        /// Tries to load game using the path provided.
        /// </summary>
        /// <param name="path">Path to game save file.</param>
        /// <param name="gameName">Game name to display on error.</param>
        /// <returns>Whether loading was successful.</returns>
        private bool TryLoadAndInitGame(string path, string gameName)
        {
            GameEnvironment loaded = TryOpenGame(path, gameName);
            if (loaded == null)
                return false;
            if (!atGameState)
            {
                DisposeUnitTableView();
                InitializeGameTableViewAppearance();
                atGameState = true;
            }
            else
                dgvUnitTable.Rows.Clear();
            game = loaded;
            AppendPlayingUnitRow(game.First, "You");
            AppendPlayingUnitRow(game.Second, "Enemy");
            game.OnGameFinished += OnGameFinished;
            SetInfoStatus("Game loaded.");
            return true;
        }

        /// <summary>
        /// Tries to autosave current game state. Doesn't throw exceptions.
        /// </summary>
        private void TryAutosave()
        {
            try
            {
                var stream = new FileStream(autosavePath, FileMode.Create, FileAccess.Write, FileShare.Read);
                SaveCurrentGame(stream);
                stream.Close();
                stream.Dispose();
            }
            catch
            {

            }
        }

        /// <summary>
        /// Actions if game action (i.e. attack, run, etc.) tool strip button is clicked.
        /// </summary>
        private void GameActionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            game.First.State = (PlayingUnit.UnitState)((ToolStripMenuItem)sender).Tag;
            SetRandomState(game.Second);
            game.Iteration();
            dgvUnitTable.Rows[0].Cells[healthCollumnIndex].Value = game.First.Health;
            dgvUnitTable.Rows[1].Cells[healthCollumnIndex].Value = game.Second.Health;
            if (game.Result != GameEnvironment.GameResult.InProcess)
            {
                DisposeGameTableView();
                InitializeValueFilterRow();
                foreach (var kvPair in units)
                    AppendRow(kvPair.Value);
                playGameToolStripMenuItem.Visible = true;
            }
            else
            {
                TryAutosave();
                SetInfoStatus(game.GetStatusString());
            }
        }

        /// <summary>
        /// Saves current game to the stream provided.
        /// </summary>
        private void SaveCurrentGame(Stream s)
        {
            XmlWriter writer = XmlWriter.Create(s);
            var serializer = new XMLPropertyClassSerializer<GameEnvironment>();
            serializer.SerializeTo(writer, game, "root");
            writer.Dispose();
        }

        /// <summary>
        /// Opens the game from the stream provided.
        /// </summary>
        private GameEnvironment OpenGame(Stream s)
        {
            XmlReader reader = XmlReader.Create(s);
            var serializer = new XMLPropertyClassSerializer<GameEnvironment>();
            return serializer.DeserializeFrom(reader, "root");
        }

        /// <summary>
        /// Tries to open the game using path provided.
        /// </summary>
        /// <param name="path">Path to game save file.</param>
        /// <param name="gameName">Game name to display on error.</param>
        private GameEnvironment TryOpenGame(string path, string gameName)
        {
            GameEnvironment gameLoaded = null;
            FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            try
            {
                gameLoaded = OpenGame(stream);
            }
            catch (IOException ex)
            {
                SetErrorStatus($"Unable to load {gameName} due to IO error: {ex.Message}");
            }
            catch (XmlException ex)
            {
                SetErrorStatus($"Unable to read {gameName} due to broken file: {ex.Message}");
            }
            catch (KeyNotFoundException)
            {
                SetErrorStatus($"Unable to find game XML element.");
            }
            catch (TargetInvocationException ex)
            {
                if (ex.InnerException is TargetInvocationException)
                {
                    if (ex.InnerException.InnerException is ArgumentException)
                        SetErrorStatus($"Unable to read {gameName} due to invalid value ranges.");
                }
                else if (ex.InnerException is Exception && ex.InnerException.InnerException is FormatException)
                    SetErrorStatus($"Unable to read {gameName} due to incorrect value format.");
                else
                    SetErrorStatus($"Unable to read {gameName}.");

            }
            catch (Exception)
            {
                SetErrorStatus($"Unable to read {gameName}.");
            }
            try
            {
                stream.Close();
                stream.Dispose();
            }
            catch { }
            return gameLoaded;
        }

        /// <summary>
        /// Actions to perform on save game tool strip button click.
        /// </summary>
        private void SaveGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.DefaultExt = ".xml";
            saveFileDialog.Filter = "XML Files (*.xml)|*.xml";
            saveFileDialog.Title = "Save current game";
            saveFileDialog.AddExtension = true;
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    SaveCurrentGame(saveFileDialog.OpenFile());
                }
                catch
                {
                    SetErrorStatus("Unable to save file at the path specified.");
                    return;
                }
                SetInfoStatus("File saved");
            }
            
        }

        /// <summary>
        /// Actions to perform on save game tool strip button click.
        /// </summary>
        private void LoadGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = false;
            openFileDialog.AddExtension = false;
            openFileDialog.CheckFileExists = true;
            openFileDialog.CheckPathExists = true;
            openFileDialog.DefaultExt = ".xml";
            openFileDialog.Filter = "XML Files (*.xml)|*.xml|All files (*)|*";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
                TryLoadAndInitGame(openFileDialog.FileName, "game");
        }

        /// <summary>
        /// Actions to perform when this from is shown. Checks for autosave and ask user whether to load it.
        /// </summary>
        private void MainForm_Shown(object sender, EventArgs e)
        {
            if (File.Exists(autosavePath))
            {
                if (MessageBox.Show("Autosave found. Would you like to load it?", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    TryLoadAndInitGame(autosavePath, "autosave");
            }
        }

        /// <summary>
        /// Actions to perform on form resize.
        /// </summary>
        private void MainForm_Resize(object sender, EventArgs e)
        {
            dgvUnitTable.Size += Size - oldSize;
            oldSize = Size;
        }

        /// <summary>
        /// Actions to perform on form resize begin.
        /// </summary>
        private void MainForm_ResizeBegin(object sender, EventArgs e)
        {
            dgvUnitTable.Visible = false;
        }

        /// <summary>
        /// Actions to perform on form resize end.
        /// </summary>
        private void MainForm_ResizeEnd(object sender, EventArgs e)
        {
            dgvUnitTable.Size += Size - oldSize;
            oldSize = Size;
            dgvUnitTable.Visible = true;
        }
    }
}
