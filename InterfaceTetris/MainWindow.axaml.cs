/* Fichier MainWindow.axaml.cs
 * Gère l'interface du jeu de Tetris : la fenêtre graphique et 
 * l'ensemble des interactions du jeu.
 * Auteur : Groupe 5.
 * Membres : ALSHAMAS Ghaith, BARROIS Nathan, DENNEMONT Maël, DIOP Awa.
 * Version : 1.3
 */

using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using System;
using Avalonia.Threading;
using NoyauTetris;

namespace InterfaceTetris;

/** Gère la fenêtre principale du jeu de Tetris, et l'ensemble des interactions du jeu. */
public partial class MainWindow : Window
{
    /* Minuteur qui déclanche régulièrement un évènement. */
    public DispatcherTimer Minuteur;
    // Définit l'attribut Jeu de type JeuTetris
    public JeuTetris Jeu;

    public MainWindow()
    {
        // Initialise l'attribut Jeu
        Jeu = new JeuTetris();

        InitializeComponent();
        // Défini la taille de la fenêtre à partir des constantes
        //LargeurFenetre =
        //LargeurCanvas + 2×Cadre + 2×Marges
        //= 264 + 24 + 80
        //= 368 px
        Width = 368;
        //HauteurFenetre =
        //330 + 24 + 80 + 40 + 40 + 40 + 40
        Height = 594;
        // Définit le texte de InfoText
        InfoText.Text = "Zone texte";
        // Défini la taille du canvas à partir des constantes
        TetrisCanvas.Width = JeuTetris.LargeurGrille * 22 + 16; //+16 pour les bordures noires : 8 à gauche et 8 à droite
        TetrisCanvas.Height = JeuTetris.HauteurGrille * 22 + 8;//+8 pour la bordure noire de bas
        // Défini la taille des boutons à partir des constantes
        StartButton.Width = TetrisCanvas.Width;
        StartButton.Height = 30;
        QuitButton.Width = TetrisCanvas.Width;
        QuitButton.Height = 30;
        // Initialise le minuteur pour faire descendre le tetrino courant toutes les 500 milisecondes
        Minuteur = new DispatcherTimer();
        Minuteur.Interval = TimeSpan.FromMilliseconds(500);
        Minuteur.Tick += (s, e) => { BasInterface(); };
        // détecte le clic sur le bouton Démarrer, déclanche l'évènement Demarrer, puis appelle la méthode DemarrerTetris
        StartButton.Click += (s, e) => { DemarrerInterface(); };
        // détecte le clic sur le bouton Quitter, déclanche l'évènement Quiter, puis ferme la fenêtre
        QuitButton.Click += (s, e) => { Close(); };
        // détecte la pression d'une touche du clavier, et déclanche l'évènement correspondant
        KeyDown += (s, e) =>
        {
            // Choix des touches à modifier si besoin (voir la documentation de l'énumération Key)
            if (e.Key == Key.Left)
            {
                GaucheInterface();
            }
            else if (e.Key == Key.Right)
            {
                DroiteInterface();
            }
            else if (e.Key == Key.X)
            // si vous disposer d'un pavé numérique, choisir Key.PageUp
            {
                RotationDroiteInterface();
            }
            else if (e.Key == Key.W)
            // si vous disposer d'un pavé numérique, choisir Key.Home
            {
                RotationGaucheInterface();
            }
            else if (e.Key == Key.Down)
            {
                TombeInterface();
            }
        };
    }


    /** Dessine un rectangle dans le TetrisCanvas, à la position (x, y), de largeur width, 
    de hauteur height (en pixels) et de couleur couleur. */
    public void DessinerRectangle(int x, int y, int with, int height, Avalonia.Media.IBrush couleur)
    {
        TetrisCanvas.Children.Add(new Avalonia.Controls.Shapes.Rectangle
        {
            Width = with,
            Height = height,
            Fill = couleur,
            Margin = new Thickness(x, y, 0, 0)
        });
    }

    /** Dessiner un cadre blanc dans le TetrisCanvas pour pouvoir initialiser le jeu sur un fond blanc */
    public void DessinerCadre()
    {
        /*On dessine un rectangle noir qui commence au coin haut gauche et va jusqu'au coin bas droit.
        Dedans, on dessine un rectangle blanc pour avoir le même résultat que la Figure 2.*/
        DessinerRectangle(0, 0, (int)TetrisCanvas.Width, (int)TetrisCanvas.Height, ConvertirCouleur(TetrinoCouleur.Noir));
        DessinerRectangle(8, 0, (int)TetrisCanvas.Width - 16, (int)TetrisCanvas.Height - 8, ConvertirCouleur(TetrinoCouleur.Blanc));
    }

    /**  prend en argument les coordonnées du carré
    (dans le repère du jeu (RJ) en nombre de carrés) et sa couleur ( TetrinoCouleur) et dessine le carré
    voulu dans la zone graphique TetrisCanva. */
    public void DessinerCarre(int xRJ, int yRJ, TetrinoCouleur couleur)
    {
        /* On dessine un carré noir de taille 22x22, ensuite on dessine le carré coloré dedans de taille 20x20 
        Il y a 15 carraux verticalement et 12 horizontallement dont les dimensions sont 22*22
        On décale les carraux horizontals 8 pixels vers la droites à cause de la bordure noire
        Et on décale, de la même raison, les carraux verticales 8 pixels vers le bas.*/
        int largeurCarre = 22;
        int hauteurCarre = 22;
        int x = largeurCarre * xRJ + 8;
        int y = hauteurCarre * yRJ;
        // dessine le carré à partir des cordonnées x,y en pixel calculés ci-dessus
        DessinerRectangle(x, y, largeurCarre, hauteurCarre, ConvertirCouleur(TetrinoCouleur.Noir));
        DessinerRectangle(x + 1, y + 1, largeurCarre - 2, largeurCarre - 2, ConvertirCouleur(couleur));
    }

    /** Réinitialisation du cadre et dessiner le TetrinoCourant */
    public void DessinerJeu()
    {
        // Supprime le cadre et son contenu, une sorte de reset
        TetrisCanvas.Children.Clear();

        // Recréer le cadre pour le rafraichir
        DessinerCadre();

        // Prend les coordonées des carrés du Tetrino courant
        Position[] positions = Tetrino.TetrinosTab[Jeu.TetrinoCourant.Indice];

        foreach (Position pos in positions)
        {
            int x = Jeu.TetrinoCourant.PositionOrigine.X + pos.X;
            int y = Jeu.TetrinoCourant.PositionOrigine.Y + pos.Y;

            // Permet de prendre la forme du Tetrino courant à travers ses coordonéees
            if (y >= 0)
            {
                DessinerCarre(x, y, Jeu.TetrinoCourant.Couleur);
            }
        }
    }

    /** Lance le jeu :
       - initialise les éléments
       - démarre le minuteur */
    public void DemarrerInterface()
    {
        Jeu = new JeuTetris();   // reset jeu
        Jeu.Demarrer();          // lance le jeu
        Minuteur.Start();        // démarre le timer
        DessinerJeu();           // affiche
    }


    /** Déplace le Tetrimino courant vers la droite */
    public void DroiteInterface()
    {
        Jeu.Droite();
        DessinerJeu();
    }

    /** Déplace le Tetrimino courant vers la gauche */
    public void GaucheInterface()
    {
        Jeu.Gauche();
        DessinerJeu();
    }

    /** Fait descendre automatiquement le Tetrimino d'une case */
    public void BasInterface()
    {
        Jeu.Bas();
        DessinerJeu();
    }

    /** Fait descendre rapidement le Tetrimino jusqu'en bas */
    public void TombeInterface()
    {
        Jeu.Tombe();
        DessinerJeu();

    }

    /** Fait tourner le Tetrimino vers la droite */
    public void RotationDroiteInterface()
    {
        Console.WriteLine("Rotation à droit à coder...");
    }

    /** Fait tourner le Tetrimino vers la gauche */
    public void RotationGaucheInterface()
    {
        Console.WriteLine("Rotation à gauche à coder...");
    }

    /** Convertit les couleurs de type enuméré TetrinoCouleur en couleur Avalonia.Media.Brushes.COULEUR
    Retourne la couleur de type Avalonia.Media.IBrush */
    public Avalonia.Media.IBrush ConvertirCouleur(TetrinoCouleur couleur)
    {
        if (couleur == TetrinoCouleur.Blanc) return Avalonia.Media.Brushes.White;
        else if (couleur == TetrinoCouleur.Noir) return Avalonia.Media.Brushes.Black;
        else if (couleur == TetrinoCouleur.Bleu) return Avalonia.Media.Brushes.Blue;
        else if (couleur == TetrinoCouleur.Jaune) return Avalonia.Media.Brushes.Yellow;
        else if (couleur == TetrinoCouleur.Cyan) return Avalonia.Media.Brushes.Cyan;
        else if (couleur == TetrinoCouleur.Vert) return Avalonia.Media.Brushes.Green;
        else if (couleur == TetrinoCouleur.Violet) return Avalonia.Media.Brushes.Violet;
        else if (couleur == TetrinoCouleur.Orange) return Avalonia.Media.Brushes.Orange;
        else return Avalonia.Media.Brushes.Red;
    }
}

