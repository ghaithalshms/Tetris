using System.Xml;

namespace NoyauTetris;


/** Les couleur qui seront utilisée pour les carrés */
public enum TetrinoCouleur
{
    Blanc,   // case vide
    Noir,    // bordures
    Bleu,
    Jaune,
    Rouge,
    Violet,
    Orange,
    Vert,
    Cyan,
}


/** La classe qui représante la grille du jeu Tetris avec deux champs définis dont 
les valeurs sont données dans le constructeur : LargeurGrille et HauteurGrille */
public class JeuTetris
{
    public static int LargeurGrille;
    public static int HauteurGrille;
    public Tetrino TetrinoCourant;
    // La grille qui représante les tetrinos figés : true s'il y a un carré, false sinon.
    public TetrinoCouleur[,] Grille;
    // Le constructeur, définir les tailles de la grille et le TetrinoCourant
    public JeuTetris()
    {
        LargeurGrille = 12;
        HauteurGrille = 15;
        TetrinoCourant = new Tetrino();
        // Le tableau est 2 dimensionnel : il contient LargeurGrille colonnes et HauteurGrille lignes
        // 'new bool[x, y]' crée un tableau rempli de 'false' par défaut
        this.Grille = new TetrinoCouleur[LargeurGrille, HauteurGrille];
        // Le tableau ne contient que des carrés blancs
        for (int y = 0; y < HauteurGrille; y++)
        {
            for (int x = 0; x < LargeurGrille; x++)
            {
                Grille[x, y] = TetrinoCouleur.Blanc;
            }
        }
    }

    /** Initialise le jeu avec un nouvau tetrino */
    public void Demarrer()
    {
        // Pour demarrer on a besoin d'un nouveau tetrino
        this.TetrinoCourant.NouveauTetrino();
        // réinitialiser la grille 
        this.Grille = new TetrinoCouleur[LargeurGrille, HauteurGrille];
        for (int y = 0; y < HauteurGrille; y++)
        {
            for (int x = 0; x < LargeurGrille; x++)
            {
                Grille[x, y] = TetrinoCouleur.Blanc;
            }
        }
    }

    /** Déplace d'une case vers la droite avec vérification cadre et grille */
    public void Droite()
    {
        // Ici on cherche l'indice maximale que X peut atteindre en fonction des positions sélectionnées par Indice.
        int longueurTetrino = 0;
        Position[] positions = Tetrino.TetrinosTab[this.TetrinoCourant.Indice];
        for (int i = 0; i < positions.Length; i = i + 1)
        {
            if (positions[i].X > longueurTetrino)
            {
                longueurTetrino = positions[i].X;
            }
        }
        if (LargeurGrille - 1 - (this.TetrinoCourant.PositionOrigine.X + longueurTetrino) > 0) //LargeurGrille-1 car LargeurGrille est exclu
        {
            // Vérifier si le carré à droite est blanc (vide) pour pouvoir se déplacer, pas de hors cadre grâce à la vérification ci-dessus
            if (this.Grille[this.TetrinoCourant.PositionOrigine.X + longueurTetrino, this.TetrinoCourant.PositionOrigine.Y] == TetrinoCouleur.Blanc)
            {
                this.TetrinoCourant.PositionOrigine.X += 1;
            }
        }
    }

    /** Déplace d'une case vers la gauche avec vérification cadre et grille */
    public void Gauche()
    {
        if (this.TetrinoCourant.PositionOrigine.X > 0)
        {
            // Vérifier si le carré à gauche est blanc (vide) pour pouvoir se déplacer, pas de hors cadre grâce à la vérification ci-dessus
            if (this.Grille[this.TetrinoCourant.PositionOrigine.X - 1, this.TetrinoCourant.PositionOrigine.Y] == TetrinoCouleur.Blanc)
            {
                this.TetrinoCourant.PositionOrigine.X -= 1;
            }
        }
    }


    /** Déplace d'une case vers le bas avec vérification, s'il ne peut plus descendre, le fige dans la grille, et enfin en crée un nouveau */
    public void Bas()
    {
        if (PeutDescendre())
        {
            TetrinoCourant.PositionOrigine.Y += 1;
        }
        else
        {
            this.FigerTetrino();
            this.TetrinoCourant = new Tetrino();
            this.TetrinoCourant.NouveauTetrino();
        }
    }

    /** Fait tomber le tetrino jusqu'en bas, le fige dans la grille, et enfin en crée un nouveau */
    public void Tombe()
    {
        while (PeutDescendre())
        {
            TetrinoCourant.PositionOrigine.Y += 1;
        }
        this.FigerTetrino();
        this.TetrinoCourant = new Tetrino();
        this.TetrinoCourant.NouveauTetrino();
    }

    public bool PeutDescendre()
    {
        foreach (Position pos in Tetrino.TetrinosTab[this.TetrinoCourant.Indice])
        {
            int x = TetrinoCourant.PositionOrigine.X + pos.X;
            int y = TetrinoCourant.PositionOrigine.Y + pos.Y;
            // Vérifie le bas du cadre
            if (y + 1 >= HauteurGrille)
            {
                return false;
            }
            // Vérifier si le carré n'est pas au dessus du cadre et s'il y a déjà un carré en dessous
            if (y >= 0 && Grille[x, y + 1] != TetrinoCouleur.Blanc)
            {
                return false;
            }
        }
        return true;
    }


    /** on donne y et on fait une boucle pour parcourir tout les x et si on constate y'a 
     une partie blanc alors on return false sinon la ligne est pleine  */

    public bool LignePleine(int ligne)
    {
        for (int x = 0; x < LargeurGrille; x++)
        {
            if (Grille[x, ligne] == TetrinoCouleur.Blanc)
            {
                return false;
            }
        }
        return true;
    }

    /** pour la premier partie une boucle sur une boucle pour que quand on supprime une ligne tout les lignes
     qui etait au dessus auront comme cordonne x et y+1*/
    public void SupprimerLigne(int ligne)
    {
        for (int y = ligne; y > 0; y--)
        {
            for (int x = 0; x < LargeurGrille; x++)
            {
                Grille[x, y] = Grille[x, y - 1];
            }
        }
        // On vide la première ligne
        for (int x = 0; x < LargeurGrille; x++)
        {
            Grille[x, 0] = TetrinoCouleur.Blanc;
        }
    }

    public void SupprimerLignesPleines()
    {
        for (int y = HauteurGrille - 1; y >= 0; y--)
        {
            if (LignePleine(y))
            {
                SupprimerLigne(y);
                y++; // on revérifie cette ligne après décalage
            }
        }
    }

    public void FigerTetrino()
    {
        foreach (Position pos in Tetrino.TetrinosTab[this.TetrinoCourant.Indice])
        {
            int x = TetrinoCourant.PositionOrigine.X + pos.X;
            int y = TetrinoCourant.PositionOrigine.Y + pos.Y;

            // Sécurité pour éviter les erreurs hors grille
            if (x >= 0 && x < LargeurGrille && y >= 0 && y < HauteurGrille)
            {
                Grille[x, y] = TetrinoCourant.Couleur;
            }
        }
        SupprimerLignesPleines();
    }


}


/** La classe qui définit la position d’un carré à l’aide de
    ses coordonnées X et Y dans le jeu et qui contient des méthodes pour déplacer
    les positions dans les 3 directions (gauche, droite et bas) */
public class Position
{
    public int X;
    public int Y;
    public Position(int x, int y)
    {
        this.X = x;
        this.Y = y;
    }

    /** La méthode qui déplace le carré de position X, Y à gauche. */
    public void DeplacerGauche()
    {
        this.X = this.X - 1;
    }

    /** La méthode qui déplace le carré de position X, Y à droite. */
    public void DeplacerDroite()
    {
        this.X = this.X + 1;
    }

    /** La méthode qui déplace le carré de position X, Y en bas. */
    public void DeplacerBas()
    {
        this.Y = this.Y + 1;
    }

}

/** La classe qui représante un tetrino par 4 carées (avec leur représentation par la classe Position) */
public class Tetrino
{
    // Liste des positions des carrées qui construisent les tetrinos : carré, barre horizontale et barre verticale.
    public static Position[][] TetrinosTab = new Position[][]
    {
        // Carré
        new Position[]
        {
            new Position(0,0), new Position(1,0),new Position(0,-1),new Position(1,-1),
        },
        // Barre horizontale
        new Position[]
        {
            new Position(0,0), new Position(1,0),new Position(2,0),new Position(3,0)
        },
        // Barre verticale
        new Position[]
        {
            new Position(0,0), new Position(0,-1),new Position(0,-2),new Position(0,-3)
        }
    };

    // L'indice qui désigne le quadruplet de positions dans le tableau
    public int Indice;

    // La variable qui positionne l’origine du tetrino dans le repère du jeu (coin supérieur gauche : Y=0)
    public Position PositionOrigine;

    // Méthode trouvée sur StackOverflow (questions/856154) pour avoir la longueur d'une énumération 
    // Note : ici on pourrait ne pas créer ce tableau en utilisant tout simplement l'énumération avec des casting depuis des entiers pour la génération aléatoire.
    public static TetrinoCouleur[] CouleursTetrinos = new TetrinoCouleur[Enum.GetValues(typeof(TetrinoCouleur)).Length];
    public TetrinoCouleur Couleur;
    /*  Création de l'objet random à utiliser pour génerer des nombres aléeatoires.
        Déclaration de random en tant qu'attribut pour éviter les déclarations inutiles lors de l'appelle de la méthode NouveauTetrino. */
    private static Random random = new Random();

    public Tetrino()
    {
        // Remplissage du tableau CouleursTetrinos avec un casting
        for (int i = 0; i < CouleursTetrinos.Length; i = i + 1)
        {
            CouleursTetrinos[i] = (TetrinoCouleur)i;
        }

        //Le constructeur définit un tetrino fixe : le carré rouge dont la position d’origine est (0, 0)
        this.Indice = 0; // carré 
        this.Couleur = TetrinoCouleur.Rouge;
        this.PositionOrigine = new Position(0, 0);

    }

    /** Retourner le quadruplet de positions du tetrino (dans le repère du jeu)
    Faire les calcules de positionnement à partir de la PositionOrigine */
    public Position[] Positions()
    {
        Position[] positionsAvecOrigine = new Position[TetrinosTab[Indice].Length];
        for (int i = 0; i < TetrinosTab[Indice].Length; i = i + 1)
        {
            // on défini une nouvelle Position en additionnant les deux Positions PositionOrigine et TetrinosTab[Indice][i].
            positionsAvecOrigine[i] = new Position(PositionOrigine.X + TetrinosTab[Indice][i].X, PositionOrigine.Y + TetrinosTab[Indice][i].Y);
        }
        return positionsAvecOrigine;
    }

    /** Mettre à jour le tetrino en tirant aléatoirement une Indice 
    du TetrinosTab, une positionOrigine et une Couleur en faisant la vérification du cadre.*/
    public void NouveauTetrino()
    {
        this.Indice = random.Next(0, TetrinosTab.Length);
        // Un casting a été utilisé pour définir la couleur à partir du nombre généré. On commence à 2 pour exclure les couleurs noire et blanche.
        this.Couleur = (TetrinoCouleur)random.Next(2, Enum.GetValues(typeof(TetrinoCouleur)).Length);

        // Ici on cherche l'indice maximale que X peut atteindre en fonction des positions sélectionnées par Indice.
        int longueurTetrino = 0;
        Position[] positions = TetrinosTab[this.Indice];
        for (int i = 0; i < positions.Length; i = i + 1)
        {
            if (positions[i].X > longueurTetrino) longueurTetrino = positions[i].X;
        }
        int positionOrigineX = random.Next(0, JeuTetris.LargeurGrille - longueurTetrino); // (JeuTetris.LargeurGrille - decalageMax) exclu
        this.PositionOrigine = new Position(positionOrigineX, 0);
    }

}

