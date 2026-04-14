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
    public static int TailleCarre;
    public static int TailleBordures;
    public static int TailleMerges;
    public static int TailleHauteurBouton;

    public MainWindow()
    {
        // Initialise l'attribut Jeu
        Jeu = new JeuTetris();

        TailleCarre = 20;
        TailleBordures = 10;
        TailleMerges = 50;
        TailleHauteurBouton = 36;

        InitializeComponent();
        // Définit le texte de InfoText
        InfoText.Text = "Zone texte";

        // Défini la taille du canvas à partir des constantes
        TetrisCanvas.Width = JeuTetris.LargeurGrille * TailleCarre + TailleBordures * 2; //+TailleBordures*2 pour les bordures noires : TailleBordures à gauche et TailleBordures à droite
        TetrisCanvas.Height = JeuTetris.HauteurGrille * TailleCarre + TailleBordures;//+TailleBordures pour la bordure noire de bas

        // Défini la taille des boutons à partir des constantes
        StartButton.Width = TetrisCanvas.Width;
        StartButton.Height = TailleHauteurBouton;
        QuitButton.Width = TetrisCanvas.Width;
        QuitButton.Height = TailleHauteurBouton;

        // Width = TetrisCanvas.Width + (2 * Marges )
        Width = TetrisCanvas.Width + TailleMerges * 2;
        // Height = TetrisCanvas.Height + Boutons + (2 * Marges)
        Height = TetrisCanvas.Height + TailleHauteurBouton * 2 + TailleMerges * 2;

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
        DessinerRectangle(TailleBordures, 0, (int)TetrisCanvas.Width - TailleBordures * 2, (int)TetrisCanvas.Height - TailleBordures, ConvertirCouleur(TetrinoCouleur.Blanc));
    }

    /**  prend en argument les coordonnées du carré
    (dans le repère du jeu (RJ) en nombre de carrés) et sa couleur ( TetrinoCouleur) et dessine le carré
    voulu dans la zone graphique TetrisCanva. */
    public void DessinerCarre(int xRJ, int yRJ, TetrinoCouleur couleur)
    {
        /* On dessine un carré noir de taille TailleCarre*TailleCarre, ensuite on dessine le carré coloré dedans de taille (TailleCarre-2)*(TailleCarre*2) 
        Il y a JeuTetris.HauteurGrille carraux verticalement et JeuTetris.LargeurGrille horizontallement dont les dimensions sont TailleCarre*TailleCarre
        On décale les carraux horizontals TailleBordures pixels vers la droites à cause de la bordure noire
        Et on décale, de la même raison, les carraux verticales TailleBordures pixels vers le bas.*/

        int x = TailleCarre * xRJ + TailleBordures;
        int y = TailleCarre * yRJ;
        // dessine le carré à partir des cordonnées x,y en pixel calculés ci-dessus
        DessinerRectangle(x, y, TailleCarre, TailleCarre, ConvertirCouleur(TetrinoCouleur.Noir));
        DessinerRectangle(x + 1, y + 1, TailleCarre - 2, TailleCarre - 2, ConvertirCouleur(couleur));
    }

    /** Réinitialisation du cadre et dessiner le TetrinoCourant et la Grille */
    public void DessinerJeu()
    {
        // Supprime le cadre et son contenu, une sorte de reset
        TetrisCanvas.Children.Clear();

        // Recréer le cadre pour le rafraichir
        DessinerCadre();

        // Dessiner la grille (les tétrinos figés)
        for (int y = 0; y < JeuTetris.HauteurGrille; y++)
        {
            for (int x = 0; x < JeuTetris.LargeurGrille; x++)
            {
                // Si la case n'est pas blanche, on dessine un carré de la couleur correspondante
                if (JeuTetris.Grille[x, y] != TetrinoCouleur.Blanc)
                {
                    DessinerCarre(x, y, JeuTetris.Grille[x, y]);
                }
            }
        }

        //Prend les coordonées des carrés du Tetrino courant en prenant en compte l'origine
        Position[] positions = Jeu.TetrinoCourant.Positions();

        foreach (Position pos in positions)
        {
            // Ne dessiner que les carrés qui sont dans le cadre
            if (pos.Y >= 0)
            {
                // Permet de prendre la forme du Tetrino courant à travers ses coordonéees
                DessinerCarre(pos.X, pos.Y, Jeu.TetrinoCourant.Couleur);
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
        Jeu.TetrinoCourant.RotationDroite();
        DessinerJeu();
    }

    /** Fait tourner le Tetrimino vers la gauche */
    public void RotationGaucheInterface()
    {
        Jeu.TetrinoCourant.RotationGauche();
        DessinerJeu();
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
