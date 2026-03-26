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
    // Le constructeur, définir les tailles de la grille et le TetrinoCourant
    public JeuTetris()
    {
        LargeurGrille = 12;
        HauteurGrille = 15;
        TetrinoCourant = new Tetrino();
    }
    // Initialise le jeu avec un nouvau tetrino
    public void Demarrer()
    {
        // Pour demarrer on a besoin d'un nouveau tetrino
        this.TetrinoCourant.NouveauTetrino();
    }

    /** Déplace d'une case vers la droite avec vérification */
    public void Droite()
    {
        // Ici, on va chercher l'indice maximale que X peut atteindre en fonction des positions sélectionnées par Indice.
        int decalageMax = 0;
        Position[] positions = Tetrino.TetrinosTab[this.TetrinoCourant.Indice];
        for (int i = 0; i < positions.Length; i = i + 1)
        {
            if (positions[i].X > decalageMax)
            {
                decalageMax = positions[i].X;
            }
        }
        if (LargeurGrille - 1 - (this.TetrinoCourant.PositionOrigine.X + decalageMax) > 0) //LargeurGrille-1 car LargeurGrille est exclu
        {
            this.TetrinoCourant.PositionOrigine.X += 1;
        }
    }

    /** Déplace d'une case vers la gauche avec vérification */
    public void Gauche()
    {
        if (this.TetrinoCourant.PositionOrigine.X > 0)
        {
            this.TetrinoCourant.PositionOrigine.X -= 1;
        }
    }


    /** Déplace d'une case vers le bas avec vérification
     Si le tetrino arrive en bas, il disparaît et un nouveau apparaît */
    public void Bas()
    {
        if (this.TetrinoCourant.PositionOrigine.Y < HauteurGrille - 1)
        {
            this.TetrinoCourant.PositionOrigine.Y += 1; // HauteurGrille-1 car HauteurGrille est exclu
        }
        else
        {
            Demarrer();
        }
    }
    /** Fait tomber le tetrino jusqu'en bas, puis en crée un nouveau */
    public void Tombe()
    {
        while (this.TetrinoCourant.PositionOrigine.Y < HauteurGrille - 1)
        {
            Bas();
        }
        Demarrer();
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
    // Note : ici on pourrait ne pas créer ce tableau en utilisant tout simplement l'énumération avec des casting depuis des entiers pour les choix
    public static TetrinoCouleur[] CouleursTetrinos = new TetrinoCouleur[Enum.GetValues(typeof(TetrinoCouleur)).Length];
    public TetrinoCouleur Couleur;
    /*  Création de l'objet random à utiliser pour génerer des nombres 'aléeatoires'.
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
    du TetrinosTab, une positionOrigine et une Couleur */
    public void NouveauTetrino()
    {
        this.Indice = random.Next(0, TetrinosTab.Length);
        // Un casting a été utilisé pour définir la couleur à partir du nombre généré. On commence à 2 pour exclure les couleurs noire et blanche.
        this.Couleur = (TetrinoCouleur)random.Next(2, Enum.GetValues(typeof(TetrinoCouleur)).Length);

        // Ici, on va chercher l'indice maximale que X peut atteindre en fonction des positions sélectionnées par Indice.
        int decalageMax = 0;
        Position[] positions = TetrinosTab[this.Indice];
        for (int i = 0; i < positions.Length; i = i + 1)
        {
            if (positions[i].X > decalageMax) decalageMax = positions[i].X;
        }
        int positionOrigineX = random.Next(0, JeuTetris.LargeurGrille - decalageMax); // (JeuTetris.LargeurGrille - decalageMax) exclu
        this.PositionOrigine = new Position(positionOrigineX, 0);
    }

}

