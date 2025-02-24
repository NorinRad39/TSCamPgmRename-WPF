using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using TopSolid.Kernel.Automating;
using TopSolid.Cad.Design.Automating;
using TopSolid.Cam.NC.Kernel.Automating;
using TSH = TopSolid.Kernel.Automating.TopSolidHost;
using TSHD = TopSolid.Cad.Design.Automating.TopSolidDesignHost;
using TSCH = TopSolid.Cam.NC.Kernel.Automating.TopSolidCamHost;
using Wpf = System.Windows;


namespace TSCamPgmRename_WPF
{
    public class Document
    {
        // Identifiant du document
        public DocumentId DocId { get; set; }

        // Liste des opérations du document
        public List<ElementId> Operations => GetOperations(DocId);

        // Liste des opérations CAM si le document est un document CAM
        public List<ElementId> CamOperations => GetCamOperations(DocId);

        // Liste des paramètres associés au document
        public List<ElementId> Parametres => GetDocuParameters(DocId);

        // Nom du document
        public string Nom => GetNomDocu(DocId);

        // Extension du document (ex: .TopPrt, .TopAsm)
        public string Extension => GetExtension(PdmObject);

        // Vérifie si le document est un document CAM
        public bool DocuCam => IsDocuCam(DocId);

        // Récupère l'identifiant PDM du document
        public PdmObjectId PdmObject => PdmObjectDocu(DocId);

        // Numéro OP du document (si applicable)
        public string OP => NumOP(DocId);

        // Cache pour stocker les noms des documents déjà récupérés afin d'éviter des appels répétés à TopSolid
        private static Dictionary<DocumentId, string> _cacheNomDocuments = new Dictionary<DocumentId, string>();

        // Constructeur par défaut
        public Document() { }

        // Constructeur avec initialisation de l'identifiant du document
        public Document(DocumentId docId)
        {
            DocId = docId;
        }

        /// <summary>
        /// Vérifie si un DocumentId est valide (non vide)
        /// </summary>
        private bool IsValidDocument(DocumentId document)
        {
            if (document == DocumentId.Empty)
            {
                Wpf.MessageBox.Show("Erreur : DocumentId vide.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Gère les exceptions et affiche un message d'erreur
        /// </summary>
        private void HandleException(Exception ex, string message)
        {
            LogError(message, ex);
            Wpf.MessageBox.Show($"{message}\nDétails : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        /// Enregistre les erreurs dans un fichier log
        /// </summary>
        private void LogError(string message, Exception ex)
        {
            string logPath = "errors.log";
            File.AppendAllText(logPath, $"{DateTime.Now}: {message} - {ex.Message}\n");
        }

        /// <summary>
        /// Récupère la liste des opérations d'un document
        /// </summary>
        private List<ElementId> GetOperations(DocumentId document)
        {
            if (!IsValidDocument(document)) return new List<ElementId>();

            try
            {
                return TSH.Operations.GetOperations(document);
            }
            catch (Exception ex)
            {
                HandleException(ex, "Impossible de récupérer les opérations.");
                return new List<ElementId>();
            }
        }

        /// <summary>
        /// Récupère les opérations CAM uniquement si le document est un document CAM
        /// </summary>
        private List<ElementId> GetCamOperations(DocumentId document)
        {
            if (!IsValidDocument(document) || !TSCH.Documents.IsCam(document))
                return new List<ElementId>();

            try
            {
                return TSCH.Operations.GetOperations(document);
            }
            catch (Exception ex)
            {
                HandleException(ex, "Impossible de récupérer les opérations CAM.");
                return new List<ElementId>();
            }
        }

        /// <summary>
        /// Récupère la liste des paramètres associés au document
        /// </summary>
        private List<ElementId> GetDocuParameters(DocumentId document)
        {
            if (!IsValidDocument(document)) return new List<ElementId>();

            try
            {
                return TSH.Parameters.GetParameters(document);
            }
            catch (Exception ex)
            {
                HandleException(ex, "Impossible de récupérer les paramètres.");
                return new List<ElementId>();
            }
        }

        /// <summary>
        /// Récupère le nom du document et le met en cache pour éviter les appels répétitifs
        /// </summary>
        private string GetNomDocu(DocumentId document)
        {
            if (!IsValidDocument(document)) return string.Empty;

            // Vérifie si le nom du document est déjà en cache
            if (_cacheNomDocuments.ContainsKey(document))
                return _cacheNomDocuments[document];

            try
            {
                string nom = TSH.Documents.GetName(document);
                _cacheNomDocuments[document] = nom; // Stocke en cache
                return nom;
            }
            catch (Exception ex)
            {
                HandleException(ex, "Impossible de récupérer le nom du document.");
                return string.Empty;
            }
        }

        /// <summary>
        /// Récupère l'extension du document (ex: .TopPrt, .TopAsm)
        /// </summary>
        private string GetExtension(PdmObjectId pdmObject)
        {
            string extension = string.Empty;
            try
            {
                TSH.Pdm.GetType(pdmObject, out extension);
                return extension;
            }
            catch (Exception ex)
            {
                HandleException(ex, "Impossible de récupérer l'extension du document.");
                return string.Empty;
            }
        }

        /// <summary>
        /// Vérifie si le document est un document CAM
        /// </summary>
        private bool IsDocuCam(DocumentId document)
        {
            if (!IsValidDocument(document)) return false;

            return TSCH.Documents.IsCam(document);
        }

        /// <summary>
        /// Récupère l'identifiant PDM du document
        /// </summary>
        private PdmObjectId PdmObjectDocu(DocumentId document)
        {
            if (!IsValidDocument(document)) return new PdmObjectId();

            try
            {
                return TSH.Documents.GetPdmObject(document);
            }
            catch (Exception ex)
            {
                HandleException(ex, "Impossible de récupérer l'ID PDM du document.");
                return new PdmObjectId();
            }
        }

        /// <summary>
        /// Récupère le numéro OP du document (si applicable)
        /// </summary>
        private string NumOP(DocumentId document)
        {
            if (!IsValidDocument(document)) return string.Empty;

            try
            {
                ElementId OP = TSH.Elements.SearchByName(document, "OP");
                return OP != ElementId.Empty ? TSH.Parameters.GetTextValue(OP) : string.Empty;
            }
            catch (Exception ex)
            {
                HandleException(ex, "Impossible de récupérer le numéro OP.");
                return string.Empty;
            }
        }
    }
}
