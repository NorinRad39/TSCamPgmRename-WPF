using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using TopSolid.Kernel.Automating;
using TopSolid.Cad.Design.Automating;
using TopSolid.Cad.Drafting.Automating;
using TopSolid.Cam.NC.Kernel.Automating;
using TSH = TopSolid.Kernel.Automating.TopSolidHost;
using TSHD = TopSolid.Cad.Design.Automating.TopSolidDesignHost;
using System.Diagnostics;
using TSCH = TopSolid.Cam.NC.Kernel.Automating.TopSolidCamHost;
using S = System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Reflection.Emit;
using Wpf = System.Windows;



namespace TSCamPgmRename_WPF
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Document currentDoc;
        private StartConnect startConnect;
        bool elec = new bool();
        List<ElementId> programs = new List<ElementId>();


        public MainWindow()
        {
            InitializeComponent();
            startConnect = new StartConnect();
            startConnect.ConnectionTopsolid();
            InitializeForm();
        }
        private void InitializeForm()
        {
            // Initialisation de currentDoc
            currentDoc = new Document();
            currentDoc.DocId = TSH.Documents.EditedDocument;

            programs = ListePrograms(currentDoc.CamOperations);

            if (programs.Count == 0)
            {
                Wpf.MessageBox.Show("Aucun programme CN trouvé pour les opérations du document.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close(); 

            }

            // Mettre à jour le texte du label avec le nom du document
            label2.Text = currentDoc.Nom;

            elec = NommagePgmElec();
            NommagePgm(elec);

            // Obtenir le nom du programme avec l'OP
            string nomPgm = NomPgmAvecOp(currentDoc.OP);
            UpdateTextBox1();

            string nomMachine = DocuMachine(currentDoc.Operations);
        }

        private void Actualiser_Click(object sender, EventArgs e)
        {
            InitializeForm();
        }

        private void Renommer_Click(object sender, EventArgs e)
        {
            

            List<string> programsNames = ProgramsNameList(programs);
            if (elec)
            {
                RenommeProgramsElec(programs, textBox1.Text);
            }
            else
            {
                RenommePrograms(programs);
            }

            Wpf.MessageBox.Show("Operation terminée", "Information", MessageBoxButton.OK, MessageBoxImage.Information);

        }
        /// <summary>
        /// Construction numero de programme par defaut
        /// </summary>
        /// <param name="op"> String qui contient le numero d'OP du document courant</param>
        /// <returns>string nomPgm qui represente le numero de programme par defaut</returns>
        private string NomPgmAvecOp(string op)
        {
            string nomPgm = string.Empty;
            if (op != string.Empty)
            {
                return nomPgm = op + "000";
            }
            return nomPgm;
        }

        /// <summary>
        /// Récuperation des programmes associé au operations
        /// </summary>
        /// <param name="camOperations"></param>
        /// <returns>liste ElementId des programmes CN</returns>
        private List<ElementId> ListePrograms(List<ElementId> camOperations)
        {
            //déclaration de la liste des programmes lié a l'operations
            List<ElementId> programsOpsListe = new List<ElementId>();

            //déclaration du hashset de verification. l'elementId des programmes seront unique.
            HashSet<ElementId> UniqueOperationExId = new HashSet<ElementId>();

            foreach (ElementId camOperation in camOperations)
            {
                ElementExId operationExId = new ElementExId(camOperation);
                ElementId programId = TSCH.Programs.GetProgram(operationExId);

                AddUniqueWord(programsOpsListe, UniqueOperationExId, programId);
            }
            return programsOpsListe;
        }

        /// <summary>
        /// verifie si l'elementId est unique avant d'ajouter a la liste des programmes
        /// </summary>
        /// <param name="programsOpsListe"></param>
        /// <param name="UniqueOperationExId"></param>
        /// <param name="programId"></param>
        static void AddUniqueWord(List<ElementId> programsOpsListe, HashSet<ElementId> UniqueOperationExId, ElementId programId)
        {
            if (UniqueOperationExId.Add(programId))
            {
                programsOpsListe.Add(programId);
            }
        }

        /// <summary>
        /// Ajoute le nom PGM suivant l'OP a textbox3 et l'increment 5 a textbox2
        /// </summary>
        /// <param name="elec"></param>
        private void NommagePgm(bool elec)
        {
            if (!elec)
            {
                

                // Obtenir le nom du programme avec l'OP
                string nomPgm = NomPgmAvecOp(currentDoc.OP);

                // Mettre à jour le texte du TextBox avec le nom du programme
                textBox3.Text = nomPgm;

                //Increment par defaut
                textBox2.Text = "5";
            }
        }

        /// <summary>
        /// recupere les noms des programmes
        /// </summary>
        /// <param name="listePrograms"></param>
        /// <returns>Renvoie la liste des operations d'usinage CN</returns>
        private List<string> ProgramsNameList(List<ElementId> listePrograms)
        {
            try
            {
                //Recupere nom des programmes du documents courant
                List<string> programNames = new List<string>();

                foreach (ElementId Programs in listePrograms)
                {
                    string programName = TopSolidCamHost.Programs.GetName(Programs);
                    programNames.Add(programName);
                    //MessageBox.Show(programName);
                }
                return programNames;
            }
            catch (Exception ex)
            {
                Wpf.MessageBox.Show("Erreur : " + ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return new List<string>();
            }
        }

        #region Fonction du bouton renommer
        /// <summary>
        /// Renomme les programmes CN quand ce n'est pas une electrode
        /// </summary>
        /// <param name="listePrograms"></param>
        private void RenommePrograms(List<ElementId> listePrograms)
        {
            string prefix = textBox1.Text;

            var (numeroProgramme, isNumeroProgrammeValid) = TextBoxToInt(textBox3.Text);
            var (increment, isIncrementValid) = TextBoxToInt(textBox2.Text);

            if (!isNumeroProgrammeValid || !isIncrementValid)
            {
                Wpf.MessageBox.Show("Erreur : Les valeurs de textBox2 et textBox3 doivent être des entiers valides.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            int nouveauNom = numeroProgramme;
            string nouveauNomTxt = nouveauNom.ToString();
            string error = "";

            if (!TSH.Application.StartModification("My Action", false)) return;
            try
            {
                foreach (ElementId Programs in listePrograms)
                {
                    string nomfinal = prefix + nouveauNomTxt;
                    TSCH.Programs.SetName(Programs, nomfinal, out error);
                    nouveauNom += increment;
                    nouveauNomTxt = nouveauNom.ToString();
                }
                TSH.Application.EndModification(true, true);
            }
            catch
            {
                // End modification (failure).
                TSH.Application.EndModification(false, false);
            }
        }

        /// <summary>
        ///Renomme les programmes CN quand c'est une electrode
        /// </summary>
        /// <param name="listePrograms"></param>
        /// <param name="nomProgElec"></param>
        private void RenommeProgramsElec(List<ElementId> listePrograms, string nomProgElec)
        {
            string error = "";

            if (!TSH.Application.StartModification("My Action", false)) return;
            try
            {
                foreach (ElementId Programs in listePrograms)
                {
                    TSCH.Programs.SetName(Programs, nomProgElec, out error);
                    TSH.Application.EndModification(true, true);
                }
            }
            catch
            {
                // End modification (failure).
                TSH.Application.EndModification(false, false);
            }
        }

        /// <summary>
        ///Verifie si le texte est numerique
        /// </summary>
        /// <param name="textBoxValue"></param>
        /// <returns></returns>
        private (int, bool) TextBoxToInt(string textBoxValue)
        {
            if (int.TryParse(textBoxValue, out int textBoxInt))
            {
                return (textBoxInt, true);
            }
            else
            {
                Wpf.MessageBox.Show("Erreur : La valeur entrée n'est pas un entier valide.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return (0, false); // Retourner une valeur par défaut et un booléen indiquant l'échec
            }
        }
        #endregion


        #region Verification machine et edit textBox1 avec O
        /// <summary>
        /// Recupere le nom de la machine
        /// </summary>
        /// <param name="operations"></param>
        /// <returns>nomMachine = nom de la machine</returns>
        private string DocuMachine(List<ElementId> operations)
        {
            if (operations.Count <= 0)
            {
                return string.Empty;
            }

            foreach (ElementId Operation in operations)
            {
                bool isInclusion = TSHD.Assemblies.IsInclusion(Operation);

                if (isInclusion)
                {
                    DocumentId documentMachine = TSHD.Assemblies.GetInclusionDefinitionDocument(Operation);
                    string nomMachine = TSH.Documents.GetName(documentMachine);
                    return nomMachine;
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// verifie si la machine est la mori
        /// </summary>
        /// <param name="nomMachine"></param>
        /// <returns>isMori = un bool</returns>
        private bool IsMori(string nomMachine)
        {
            bool isMori = false;

            if (nomMachine.StartsWith("MORI"))
            {
                return isMori = true;
            }

            return isMori;
        }

        /// <summary>
        /// edit le textBox1 avec O si c'est la mori
        /// </summary>
        private void UpdateTextBox1()
        {
           

            string nomMachine = DocuMachine(currentDoc.Operations);

            bool isMori = IsMori(nomMachine);

            if (isMori)
            {
                textBox1.Text = "O";

            }
        }
        #endregion

        #region Nom electrode
        /// <summary>
        /// Recupere la valeur du parametre nom elec si present
        /// </summary>
        /// <param name="parametres"></param>
        /// <returns>nomElectrode = le nom de l'electrode</returns>
        private string RecupNomElectrode(List<ElementId> parametres)
        {
            string nomElectrode = string.Empty;

            if (parametres.Count > 0)
            {
                foreach (ElementId parametre in parametres)
                {
                    string nomParametre = TSH.Elements.GetFriendlyName(parametre);
                    //MessageBox.Show(nomParametre);

                    if (nomParametre == "Nom elec")
                    {
                        nomElectrode = TSH.Parameters.GetTextValue(parametre);
                        return nomElectrode;
                    }
                }
            }
            return nomElectrode;
        }

        /// <summary>
        /// Recupere la valeur du Gap pour construire le nom de PGM
        /// </summary>
        /// <param name="parametres"></param>
        /// <returns>gapValue = valeur du Gap en mm</returns>
        private string RecupGap(List<ElementId> parametres)
        {
            double intGapValue = 0;
            string gapValue = string.Empty;

            if (parametres.Count > 0)
            {
                foreach (ElementId parametre in parametres)
                {
                    string nomParametre = TSH.Elements.GetFriendlyName(parametre);
                    //MessageBox.Show(nomParametre);

                    if (nomParametre == "Gap")
                    {
                        intGapValue = TSH.Parameters.GetRealValue(parametre);
                        intGapValue = intGapValue * 1000;
                        gapValue = new string(intGapValue.ToString().Where(char.IsDigit).ToArray());
                        return gapValue;
                    }
                }
            }
            return gapValue;
        }
        /// <summary>
        /// Construit le nom du PGM pour electrode
        /// </summary>
        /// <returns> Le nom du PGM electrode</returns>
        private string CalculNomPgmElec()
        {

            // Accéder directement aux paramètres de currentDoc
            string nomElec = RecupNomElectrode(currentDoc.Parametres);
            string gap = RecupGap(currentDoc.Parametres);

            if (!string.IsNullOrEmpty(nomElec) && !string.IsNullOrEmpty(gap))
            {
                string nomPgmElec = nomElec + "-G" + gap;
                return nomPgmElec;
            }
            return string.Empty;
        }

        /// <summary>
        /// Ajoute le nom du PGM electrode dans textbox1
        /// </summary>
        /// <returns>Un bool pour confirmer que le nom PGM electrode est bien ajouter textbox1</returns>
        private bool NommagePgmElec()
        {
            bool elec = false;
            // Appeler CalculNomPgmElec sans paramètre
            string nomPgmElec = CalculNomPgmElec();
            if (!string.IsNullOrEmpty(nomPgmElec))
            {
                textBox1.Text = nomPgmElec;
                elec = true;
            }
            return elec;
        }

        #endregion

        #region Quitter
        private void QuitterMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        #endregion


    }
}
