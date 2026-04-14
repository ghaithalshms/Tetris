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
    public static TetrinoCouleur[,] Grille = new TetrinoCouleur[LargeurGrille, HauteurGrille];
    /*  Création de l'objet random à utiliser pour génerer des nombres aléeatoires.
    Déclaration de random en tant qu'attribut pour éviter les déclarations inutiles lors de l'appelle de la méthode NouveauTetrino. */
    private static Random random = new Random();

    // Le constructeur, définir les tailles de la grille et le TetrinoCourant
    public JeuTetris()
    {
        LargeurGrille = 10;
        HauteurGrille = 20;
        TetrinoCourant = new Tetrino();
        // Le tableau est 2 dimensionnel : il contient LargeurGrille colonnes et HauteurGrille lignes
        // 'new bool[x, y]' crée un tableau rempli de 'false' par défaut
        Grille = new TetrinoCouleur[LargeurGrille, HauteurGrille];
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
        this.NouveauTetrino();
        // réinitialiser la grille 
        Grille = new TetrinoCouleur[LargeurGrille, HauteurGrille];
        for (int y = 0; y < HauteurGrille; y++)
        {
            for (int x = 0; x < LargeurGrille; x++)
            {
                Grille[x, y] = TetrinoCouleur.Blanc;
            }
        }
    }


    /** Mettre à jour le tetrino en tirant aléatoirement une Indice 
    du TetrinosTab, une positionOrigine et une Couleur en faisant la vérification du cadre.*/
    public void NouveauTetrino()
    {
        //FIXME: Il faut vérifier si le tetrino peut loger dans la grille avant d'apparaitre
        this.TetrinoCourant.Indice = random.Next(0, Tetrino.TetrinosTab.Length);
        // Un casting a été utilisé pour définir la couleur à partir du nombre généré. On commence à 2 pour exclure les couleurs noire et blanche.
        this.TetrinoCourant.Couleur = (TetrinoCouleur)random.Next(2, Enum.GetValues(typeof(TetrinoCouleur)).Length);

        // Ici on cherche l'indice maximale que X peut atteindre en fonction des positions sélectionnées par Indice.
        int longueurTetrino = 0;
        int hauteurTetrino = 0;
        Position[] positions = Tetrino.TetrinosTab[this.TetrinoCourant.Indice];
        for (int i = 0; i < positions.Length; i = i + 1) //ici on met +1 car les indices commencent à 0
        {
            if (positions[i].X + 1 > longueurTetrino) longueurTetrino = positions[i].X + 1;
            if (positions[i].Y + 1 > hauteurTetrino) hauteurTetrino = positions[i].Y + 1;
        }
        int positionOrigineX = random.Next(0, JeuTetris.LargeurGrille - longueurTetrino + 1); // +1 car (JeuTetris.LargeurGrille - longueurTetrino) est exclu et qu'on a enlevé la longueur
        Position origine = new Position(positionOrigineX, -1); //-1 pour que le tetrino qui veut descendre puisse être vérifié s'il le peut 
        this.TetrinoCourant.PositionOrigine = origine;

        /*// Pour savoir si le tetrino peut loger, on va vérifier s'il peut descendre hauteurTetrino fois
        bool peutLoger = true;
        for (int i = 1; i <= hauteurTetrino; i++)
        {
            if (this.TetrinoCourant.Positions()[i - 1].Y + i < 0 || !PeutSeDeplacer(0, i))
            {
                peutLoger = false;
                break;
            }
        }
        if (!peutLoger)
        {
            Console.WriteLine("***** Game Over *****");
        }*/
    }



    /** Déplace d'une case vers la droite avec vérification cadre et grille */
    public void Droite()
    {
        if (this.TetrinoCourant.PeutSeDeplacer(1, 0)) this.TetrinoCourant.PositionOrigine.X += 1;
    }

    /** Déplace d'une case vers la gauche avec vérification cadre et grille */
    public void Gauche()
    {
        if (this.TetrinoCourant.PeutSeDeplacer(-1, 0)) this.TetrinoCourant.PositionOrigine.X -= 1;
    }


    /** Déplace d'une case vers le bas avec vérification, s'il ne peut plus descendre, le fige dans la grille, et enfin en crée un nouveau */
    public void Bas()
    {
        if (this.TetrinoCourant.PeutSeDeplacer(0, 1))
        {
            TetrinoCourant.PositionOrigine.Y += 1;
        }
        else
        {
            this.FigerTetrino();
            this.TetrinoCourant = new Tetrino();
            this.NouveauTetrino();
        }
    }

    /** Fait tomber le tetrino jusqu'en bas, le fige dans la grille, et enfin en crée un nouveau */
    public void Tombe()
    {
        while (this.TetrinoCourant.PeutSeDeplacer(0, 1))
        {
            TetrinoCourant.PositionOrigine.Y += 1;
        }
        this.FigerTetrino();
        this.TetrinoCourant = new Tetrino();
        this.NouveauTetrino();
    }

    /** Retourne true ou false, vérifie si la ligne est pleine.  */

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

    /** On efface la ligne d'indice donnée en tout décalant vers le bas. */
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

    /** On efface les lignes pleines. */
    public void SupprimerLignesPleines()
    {
        for (int y = HauteurGrille - 1; y >= 0; y--)
        {
            if (LignePleine(y))
            {
                SupprimerLigne(y);
                y++; // on revérifie cette ligne après décalage car le contenu a changé
            }
        }
    }

    public void FigerTetrino()
    {
        /* On fait le contrôle en commençant par la fin 
         pour savoir s'il va avoir un carré qui ne vas pas loger dans la grille */

        for (int i = Tetrino.TetrinosTab[this.TetrinoCourant.Indice].Length - 1; i >= 0; i--)
        // foreach (Position pos in Tetrino.TetrinosTab[this.TetrinoCourant.Indice])
        {
            Position pos = Tetrino.TetrinosTab[this.TetrinoCourant.Indice][i];
            int x = TetrinoCourant.PositionOrigine.X + pos.X;
            int y = TetrinoCourant.PositionOrigine.Y + pos.Y;

            // Sécurité pour éviter les erreurs hors grille
            if (x >= 0 && x < LargeurGrille && y >= 0 && y < HauteurGrille)
            {
                Grille[x, y] = TetrinoCourant.Couleur;
            }
            else
            {
                Console.WriteLine("***** Game Over *****");
                break; // si un carré est hors cadre, aucun carré ne sera ajouté à la grille, le tetrino disparait
            }
        }
        SupprimerLignesPleines();

    }


    /** Effectuer une rotation à droite si possible, en appellant la méthode RotationDroite de la classe Tetrino */
    public void RotationDroite()
    {
        int ancienIndice = TetrinoCourant.Indice;
        Position anciennePosition = new Position(
            this.TetrinoCourant.PositionOrigine.X,
            this.TetrinoCourant.PositionOrigine.Y
        );

        this.TetrinoCourant.RotationDroite();

        if (!this.TetrinoCourant.PeutSeDeplacer(0, 0))
        {
            this.TetrinoCourant.Indice = ancienIndice;
            this.TetrinoCourant.PositionOrigine.X = anciennePosition.X;
            this.TetrinoCourant.PositionOrigine.Y = anciennePosition.Y;
        }
    }

    /** Effectuer une rotation à gauche si possible, en appellant la méthode RotationGauche de la classe Tetrino */
    public void RotationGauche()
    {
        int ancienIndice = TetrinoCourant.Indice;
        Position anciennePosition = new Position(
            this.TetrinoCourant.PositionOrigine.X,
            this.TetrinoCourant.PositionOrigine.Y
        );

        this.TetrinoCourant.RotationGauche();

        if (!this.TetrinoCourant.PeutSeDeplacer(0, 0))
        {
            this.TetrinoCourant.Indice = ancienIndice;
            this.TetrinoCourant.PositionOrigine.X = anciennePosition.X;
            this.TetrinoCourant.PositionOrigine.Y = anciennePosition.Y;
        }
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

    /** Vérifier si le tetrino peut se déplacer, créée pour ne pas répeter le code dans les méthodes Droite, Gauche et Bas */
    public bool PeutSeDeplacer(int deltaX, int deltaY)
    {
        foreach (Position p in this.Positions())
        {
            int nouveauX = p.X + deltaX;
            int nouveauY = p.Y + deltaY;

            // Vérifier les bordures
            if (nouveauX < 0 || nouveauX >= JeuTetris.LargeurGrille || nouveauY >= JeuTetris.HauteurGrille)
                return false;

            // Vérifier la collision avec les tetrinos figés (si on est dans la grille, pas au dessus)
            if (nouveauY >= 0 && JeuTetris.Grille[nouveauX, nouveauY] != TetrinoCouleur.Blanc)
                return false;
        }
        return true;
    }


    /** Effectue une rotation vers la droite en mettant à jour l'indice et l'origine */
    public void RotationDroite()
    {
        // Mise à jour de l'indice (changement de forme)
        switch (this.Indice)
        {
            case 0: // Carré : ne change pas
                break;
            case 1: // Barre H -> Barre V
                this.Indice = 2;
                // Décalage spécifique pour la barre (+1, -1) 
                this.PositionOrigine.X += 1;
                this.PositionOrigine.Y -= 1;
                break;
            case 2: // Barre V -> Barre H
                this.Indice = 1;
                // Inverse du décalage pour la réversibilité (-1, +1) 
                this.PositionOrigine.X -= 1;
                this.PositionOrigine.Y += 1;
                break;
            default:
                break;
        }
    }

    /** Effectue une rotation vers la gauche (l'inverse strict de la droite) */
    public void RotationGauche()
    {
        // Mise à jour de l'indice (changement de forme)
        switch (this.Indice)
        {
            case 0: // Carré
                break;
            case 1: // Barre H -> Barre V 
                this.Indice = 2;
                this.PositionOrigine.X += 1;
                this.PositionOrigine.Y -= 1;
                break;
            case 2: // Barre V -> Barre H 
                this.Indice = 1;
                this.PositionOrigine.X -= 1;
                this.PositionOrigine.Y += 1;
                break;
            default:
                break;
        }
    }


}

