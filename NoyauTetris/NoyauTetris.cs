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

    /** Le tétrino actuellement contrôlé par le joueur */
    public Tetrino TetrinoCourant;

    /** Indique si la partie est terminée */
    public bool Perdu;

    /** Le nombre total de tétrinos générés depuis le début de la partie */
    public int NombreTetrinosApparus;

    /** Le score actuel du joueur (basé sur les lignes supprimées) */
    public int Score;

    /** La matrice représentant l'état fixe de la grille de jeu */
    public TetrinoCouleur[,] Grille;
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
        Perdu = false;
        NombreTetrinosApparus = 0;
        Score = 0;
        // réinitialiser la grille 
        this.Grille = new TetrinoCouleur[LargeurGrille, HauteurGrille];
        for (int y = 0; y < HauteurGrille; y++)
        {
            for (int x = 0; x < LargeurGrille; x++)
            {
                Grille[x, y] = TetrinoCouleur.Blanc;
            }
        }
        // Pour demarrer on a besoin d'un nouveau tetrino
        this.NouveauTetrino();
    }



    /** Mettre à jour le tetrino en tirant aléatoirement une Indice 
    du TetrinosTab, une positionOrigine et une Couleur en faisant la vérification du cadre.*/
    public void NouveauTetrino()
    {
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

        NombreTetrinosApparus++;

        foreach (Position pos in this.TetrinoCourant.Positions())
        {
            // si le tetrino ne peut pas apparaître, le jeu est fini
            if (pos.Y >= 0 && Grille[pos.X, pos.Y] != TetrinoCouleur.Blanc)
            {
                Perdu = true;
                return;
            }

        }
    }

    /** Vérifier si le tetrino peut se déplacer, créée pour ne pas répeter le code dans les méthodes Droite, Gauche et Bas */
    public bool PeutSeDeplacer(int dx, int dy)
    {
        foreach (Position pos in this.TetrinoCourant.Positions())
        {
            int x = pos.X + dx;
            int y = pos.Y + dy;

            // Sortie à gauche ou à droite ou en bas
            if (x < 0 || x >= LargeurGrille || y >= HauteurGrille)
            {
                return false;
            }
            // y < 0 autorisé tant que la pièce est encore en haut pour l'effet descente 

            if (y >= 0)
            {
                // s'il y a déjà un tétrino, on ne doit pas l'écraser
                if (Grille[x, y] != TetrinoCouleur.Blanc)
                {
                    return false;
                }
            }
        }

        return true;
    }

    /** Déplace d'une case vers la droite avec vérification cadre et grille */
    public void Droite()
    {
        if (PeutSeDeplacer(1, 0)) this.TetrinoCourant.PositionOrigine.X += 1;
    }

    /** Déplace d'une case vers la gauche avec vérification cadre et grille */
    public void Gauche()
    {
        if (PeutSeDeplacer(-1, 0)) this.TetrinoCourant.PositionOrigine.X -= 1;
    }


    /** Déplace d'une case vers le bas avec vérification, s'il ne peut plus descendre, le fige dans la grille, et enfin en crée un nouveau */

    public void Bas()
    {
        if (Perdu) return;
        if (PeutSeDeplacer(0, 1))
        {
            TetrinoCourant.PositionOrigine.Y += 1;
        }
        else
        {
            FigerTetrino();
            NouveauTetrino();
        }
    }


    /** Fait tomber le tetrino jusqu'en bas, le fige dans la grille, et enfin en crée un nouveau */
    public void Tombe()
    {
        if (Perdu) return;
        while (PeutSeDeplacer(0, 1))
        {
            TetrinoCourant.PositionOrigine.Y += 1;
        }

        FigerTetrino();
        NouveauTetrino();
    }

    /** Retourne true ou false, vérifie si la ligne est pleine.  */

    public bool LignePleine(int y)
    {
        for (int x = 0; x < LargeurGrille; x++)
        {
            if (Grille[x, y] == TetrinoCouleur.Blanc)
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

        // Ajouter 100 au Score
        this.Score += 100;
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

    /** Fige les carrés du tétrino actuel dans la grille de jeu.
        Vérifie si la pièce dépasse des limites (jeu perdu) et lance la suppression des lignes pleines. */
    public void FigerTetrino()
    {
        /* On fait le contrôle en commençant par la fin 
        pour savoir s'il va avoir un carré qui ne vas pas loger dans la grille */
        for (int i = 0; i < Tetrino.TetrinosTab[TetrinoCourant.Indice].Length; i++)
        {
            Position pos = Tetrino.TetrinosTab[TetrinoCourant.Indice][i];

            int x = TetrinoCourant.PositionOrigine.X + pos.X;
            int y = TetrinoCourant.PositionOrigine.Y + pos.Y;

            // Sécurité pour éviter les erreurs hors grille
            if (x >= 0 && x < LargeurGrille && y >= 0 && y < HauteurGrille)
            {
                Grille[x, y] = TetrinoCourant.Couleur;
            }
            else
            {
                // si le tetrino ne peut pas etre figé, alors le jeu est fini
                this.Perdu = true;
            }

        }

        SupprimerLignesPleines();
    }
    /** Effectuer une rotation à droite si possible, en appellant la méthode RotationDroite de la classe Tetrino */
    public void RotationDroite()
    {
        int ancienIndice = TetrinoCourant.Indice;
        Position anciennePosition = new Position(
            TetrinoCourant.PositionOrigine.X,
            TetrinoCourant.PositionOrigine.Y
        );

        TetrinoCourant.RotationDroite();

        if (!PeutSeDeplacer(0, 0))
        {
            TetrinoCourant.Indice = ancienIndice;
            TetrinoCourant.PositionOrigine.X = anciennePosition.X;
            TetrinoCourant.PositionOrigine.Y = anciennePosition.Y;
        }
    }
    /** Effectuer une rotation à gauche si possible, en appellant la méthode RotationGauche de la classe Tetrino */
    public void RotationGauche()
    {
        int ancienIndice = TetrinoCourant.Indice;
        Position anciennePosition = new Position(
            TetrinoCourant.PositionOrigine.X,
            TetrinoCourant.PositionOrigine.Y
        );

        TetrinoCourant.RotationGauche();

        if (!PeutSeDeplacer(0, 0))
        {
            TetrinoCourant.Indice = ancienIndice;
            TetrinoCourant.PositionOrigine.X = anciennePosition.X;
            TetrinoCourant.PositionOrigine.Y = anciennePosition.Y;
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

    /** Initialise une nouvelle instance de Position avec des coordonnées X et Y */
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

    /** Tableau statique contenant les modèles de formes pour chaque type de tetrino */
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
        },
        // Barre s
           new Position[]
        {
            new Position(0,0), new Position(0,-1),new Position(1,-1),new Position(1,-2)
        },
       
        // Barre s horizontale gauche
         new Position[]
        {
            new Position(0,0), new Position(1,0),new Position(1,-1),new Position(2,-1),
        },
        // Barre s horizontale droite
        new Position[]
        {
            new Position(1,0), new Position(2,0),new Position(0,-1),new Position(1,-1)
        },
        // Barre s verticale
        new Position[]
        {
            new Position(1,0), new Position(0,-1),new Position(1,-1),new Position(0,-2)
        },
         //Barre T
           new Position[]
        {
            new Position(0,-1), new Position(1,-1),new Position(2,-1),new Position(1,0)
        },
        // Barre T droite
           new Position[]
        {
             new Position(0,0), new Position(0,-1),new Position(0,-2),new Position(1,-1)
        },
        //Barre T  renversé
           new Position[]
        {
            new Position(0,0), new Position(1,0),new Position(2,0),new Position(1,-1)
        },
        //Barre T  gauche
           new Position[]
        {
             new Position(1,0), new Position(1,-1),new Position(1,-2),new Position(0,-1)
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

    /** Initialise un tétrino par défaut (carré rouge en haut à gauche) */
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

    /** Effectue une rotation de 90° vers la droite en modifiant l'indice et l'origine */
    public void RotationDroite()
    {
        // Indice 0 = le carré donc on ne fait rien.
        if (Indice == 0) return;

        else if (Indice == 1) // Horizontale -> Verticale
        {
            Indice = 2;
            PositionOrigine.X += 1;
            PositionOrigine.Y -= 1;
        }
        else if (Indice == 2) // Verticale -> Horizontale
        {
            Indice = 1;
            PositionOrigine.X -= 1;
            PositionOrigine.Y += 1;
        }

        else if (Indice == 3) // S vertical -> S horizontal 
        {
            Indice = 5;
            PositionOrigine.X -= 1;
            PositionOrigine.Y += 1;
        }
        else if (Indice == 5) // S horizontal -> S vertical 
        {
            Indice = 3;
            PositionOrigine.X += 1;
            PositionOrigine.Y -= 1;
        }
        else if (Indice == 4) // S horizontal -> S vertical 
        {
            Indice = 6;
            PositionOrigine.X += 1;
            PositionOrigine.Y -= 1;
        }
        else if (Indice == 6) // S vertical -> S horizontal 
        {
            Indice = 4;
            PositionOrigine.X -= 1;
            PositionOrigine.Y += 1;
        }

        else if (Indice == 7) // T pointe en bas -> T pointe à droite
        {
            Indice = 8;
            PositionOrigine.X -= 1;
            PositionOrigine.Y += 1;
        }
        else if (Indice == 8) // T pointe à droite -> T pointe en haut (renversé)
        {
            Indice = 9;
            PositionOrigine.X += 1;
            PositionOrigine.Y -= 1;
        }
        else if (Indice == 9) // T pointe en haut -> T pointe à gauche
        {
            Indice = 10;
            PositionOrigine.X -= 1;
            PositionOrigine.Y += 1;
        }
        else if (Indice == 10) // T pointe à gauche -> T pointe en bas
        {
            Indice = 7;
            PositionOrigine.X += 1;
            PositionOrigine.Y -= 1;
        }
    }

    /** Effectue une rotation de 90° vers la gauche en modifiant l'indice et l'origine */
    public void RotationGauche()
    {
        if (Indice == 0)
        {
            // Carré : aucune rotation utile
            return;
        }

        else if (Indice == 1)
        {
            // Barre horizontale -> barre verticale
            Indice = 2;
            PositionOrigine.X += 1;
            PositionOrigine.Y -= 1;
        }
        else if (Indice == 2)
        {
            // Barre verticale -> barre horizontale
            Indice = 1;
            PositionOrigine.X -= 1;
            PositionOrigine.Y += 1;
        }
        else if (Indice == 3)
        {
            // S vertical -> S horizontal
            Indice = 4;
            PositionOrigine.X -= 1;
            PositionOrigine.Y += 1;
        }
        else if (Indice == 4)
        {
            // S horizontal -> S vertical
            Indice = 3;
            PositionOrigine.X += 1;
            PositionOrigine.Y -= 1;
        }
        else if (Indice == 5)
        {
            // S horizontal -> S vertical
            Indice = 6;
            PositionOrigine.X -= 1;
            PositionOrigine.Y += 1;
        }
        else if (Indice == 6)
        {
            // S vertical -> S horizontal
            Indice = 5;
            PositionOrigine.X += 1;
            PositionOrigine.Y -= 1;
        }

        else if (Indice == 7)
        {
            // T pointe en bas -> T pointe à gauche
            Indice = 10;
            PositionOrigine.X -= 1;
            PositionOrigine.Y += 1;
        }


        else if (Indice == 8)
        {
            // T pointe à droite -> T pointe en bas 
            Indice = 7;
            PositionOrigine.X += 1;
            PositionOrigine.Y -= 1;
        }
        else if (Indice == 9)
        {
            // T pointe en haut -> T pointe à droite
            Indice = 8;
            PositionOrigine.X -= 1;
            PositionOrigine.Y += 1;
        }
        else if (Indice == 10)
        {
            // T pointe à gauche -> T pointe en haut (renversé)
            Indice = 9;
            PositionOrigine.X += 1;
            PositionOrigine.Y -= 1;
        }
    }

}
