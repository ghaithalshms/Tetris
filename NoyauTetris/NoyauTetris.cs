namespace NoyauTetris;


/** Les couleur qui seront utilisée pour les carraux 
    NOTE IMPORTANTE : cet enumération a été déplacé ici, dans le noyeau, car est utile dans ce fichier.
    Comme le l'interface est dépendante du noyeau et a un lien avec, cela ne pose pas de problème. */
public enum TetrinoCouleur
{
    Aucune,   // case vide
    Cadre,    // bordures
    Bleu,
    Jaune,
    Rouge,
    Violet,
    Orange,
    Vert,
    Cyan,
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
        
    }

    /** La méthode qui déplace le carré de position X, Y à droite. */
    public void DeplacerDroite()
    {
        
    }

    /** La méthode qui déplace le carré de position X, Y en bas. */
    public void DeplacerBas()
    {
        
    }

    /** Méthode statique qui additionne deux positions du type Position. */
    public static Position AdditionPosition(Position p1, Position p2)
    {
        return new Position(p1.X + p2.X, p1.Y + p2.Y);
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

    /* !!!!!!!!!!!!!!!!!!!!!!!!!!! Pas besoin ? A DEMANDER AU PROF !!!!!!!!!!!!!!!!!!!!!!!!!!!
    // Méthode trouvée sur StacK Overflow (questions/856154) pour avoir la longueur d'une énumération 
    public static TetrinoCouleur[] CouleursTetrinos = new TetrinoCouleur[Enum.GetValues(typeof(TetrinoCouleur)).Length];*/
    public TetrinoCouleur Couleur;
    
    // Déclaration de random en tant qu'attribut pour éviter les déclarations inutiles lors de l'appelle de la méthode NouveauTetrino. 
    private static Random random = new Random();

    public Tetrino()
    {
        //this.PositionOrigine = positionOrigine;
        /*
        // Remplissage du tableau CouleursTetrinos avec un casting
        for (int i=0; i < CouleursTetrinos.Length; i=i+1) CouleursTetrinos[i] = (TetrinoCouleur)i;
        */

        //Le constructeur définit un tetrino fixe : le carré rouge dont la position d’origine est (0, 0)
        this.Indice = 0; // carré 
        this.Couleur = TetrinoCouleur.Rouge;
        this.PositionOrigine = new Position(0,0);

    }

    /** Retourner le quadruplet de positions du tetrino (dans le repère du jeu)
    Faire les calcules de positionnement à partir de la PositionOrigine */
    public Position[] Positions()
    {
        Position[] positionsAvecOrigine = new Position[TetrinosTab[Indice].Length];
        for (int i=0; i < TetrinosTab[Indice].Length; i = i + 1)
        {
            positionsAvecOrigine[i] = Position.AdditionPosition(PositionOrigine, TetrinosTab[Indice][i]);
        }
        return positionsAvecOrigine;
    }

    /** Mettre à jour le tetrino en tirant aléatoirement une Indice 
    du TetrinosTab, une positionOrigine et une Couleur */
    public void NouveauTetrino()
    {
        // création de l'objet random à utiliser pour génerer des nombres 'aléeatoires'.
        this.Indice = random.Next(0,TetrinosTab.Length);
        // Un casting a été utilisé pour définir la couleur à partir du nombre généré. On commence à 2 pour exclure les couleurs Aucune et Cadre.
        this.Couleur = (TetrinoCouleur)random.Next(2, Enum.GetValues(typeof(TetrinoCouleur)).Length);
        
        // Ici, on va chercher l'indice maximale que X peut atteindre en fonction des positions sélectionnées par Indice.
        int decalageMax = 0;
        Position[] positions = TetrinosTab[this.Indice];
        for (int i=0; i<positions.Length; i = i + 1)
        {
            if (positions[i].X > decalageMax) decalageMax = positions[i].X;
        }
        int positionOrigineX = random.Next(0,12 - decalageMax);
        this.PositionOrigine = new Position(positionOrigineX, 0);
    }

}

