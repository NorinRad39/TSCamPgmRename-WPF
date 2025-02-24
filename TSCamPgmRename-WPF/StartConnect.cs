using System;
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
    public class StartConnect
    {
        /// <summary>
        /// Établit une connexion à TopSolid.
        /// </summary>
        private void ConnectToTopSolid()
        {
            try
            {
                // Vérifier si la connexion est déjà établie
                if (!TSH.IsConnected)
                {
                    // Connexion à TopSolid avec un paramètre d'initialisation (si nécessaire)
                    TSH.Connect();

                    // Vérifier à nouveau si la connexion est réussie
                    if (TSH.IsConnected)
                    {
                        //MessageBox.Show("Connexion réussie à TopSolid.", "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        Wpf.MessageBox.Show("Connexion échouée à TopSolid.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    Wpf.MessageBox.Show("TopSolid est déjà connecté.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (InvalidOperationException ex)
            {
                // Gérer une exception spécifique si nécessaire
                Wpf.MessageBox.Show($"Problème opérationnel : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                // Gérer d'autres exceptions
                Wpf.MessageBox.Show($"Erreur lors de la connexion à TopSolid : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Établit une connexion à TopSolid Design Host.
        /// </summary>
        private void ConnectToTopSolidDesignHost()
        {
            try
            {
                // Vérifier si la connexion est déjà établie
                if (!TSHD.IsConnected)
                {
                    // Connexion à TopSolid Design Host
                    TSHD.Connect();

                    // Vérifier à nouveau si la connexion est réussie
                    if (TSHD.IsConnected)
                    {
                        //MessageBox.Show("Connexion réussie à TopSolid module design.", "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        Wpf.MessageBox.Show("Connexion échouée à TopSolid module design.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    Wpf.MessageBox.Show("TopSolid module design est déjà connecté.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (InvalidOperationException ex)
            {
                // Gérer une exception spécifique si nécessaire
                Wpf.MessageBox.Show($"Problème opérationnel : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                // Gérer d'autres exceptions
                Wpf.MessageBox.Show($"Erreur lors de la connexion à TopSolid module design : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Établit une connexion à TopSolid CAM Host.
        /// </summary>
        private void ConnectToTopSolidCamHost()
        {
            try
            {
                // Vérifier si la connexion est déjà établie
                if (!TSCH.IsConnected)
                {
                    // Connexion à TopSolid CAM Host
                    TSCH.Connect();

                    // Vérifier à nouveau si la connexion est réussie
                    if (TSCH.IsConnected)
                    {
                        //MessageBox.Show("Connexion réussie à TopSolid module CAM.", "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        Wpf.MessageBox.Show("Connexion échouée à TopSolid module CAM.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    Wpf.MessageBox.Show("TopSolid module CAM est déjà connecté.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (InvalidOperationException ex)
            {
                // Gérer une exception spécifique si nécessaire
                Wpf.MessageBox.Show($"Problème opérationnel : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                // Gérer d'autres exceptions
                Wpf.MessageBox.Show($"Erreur lors de la connexion à TopSolid module CAM : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Méthode pour connecter tous les modules TopSolid.
        /// </summary>
        public void ConnectionTopsolid()
        {
            ConnectToTopSolid();
            ConnectToTopSolidDesignHost();
            ConnectToTopSolidCamHost();
        }
    }
}
