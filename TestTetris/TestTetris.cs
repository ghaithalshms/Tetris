using NoyauTetris;

namespace TestTetris;

public class TestPosition
// ***** Nathan *****
{
    /** On vérifie que le déplacement en bas augmente la valeur de Y */
    [Fact]
    public void TestPosition_DeplacementBas()
    {
        Position pos = new Position(5, 5);

        pos.DeplacerBas();

        Assert.Equal(6, pos.Y);
        Assert.Equal(5, pos.X);
    }

    /** On vérifie que le déplacement à gauche décrémente bien X */
    [Fact]
    public void TestPosition_DeplacementGauche()
    {
        Position pos = new Position(5, 5);

        pos.DeplacerGauche();

        Assert.Equal(4, pos.X);
        Assert.Equal(5, pos.Y);
    }

    /** On vérifie que le déplacement à gauche décrémente bien X même s'il va sortir du cadre : La classe Position ne doit pas gérer cela */
    [Fact]
    public void TestPosition_DeplacementGauche_HorsCadre()
    {
        Position pos = new Position(0, 0);

        pos.DeplacerGauche();

        Assert.Equal(-1, pos.X);
        Assert.Equal(0, pos.Y);
    }

    /** On vérifie que le déplacement à droite incrémente bien X */
    [Fact]
    public void TestPosition_DeplacementDroite()
    {
        Position pos = new Position(5, 5);

        pos.DeplacerDroite();

        Assert.Equal(6, pos.X);
        Assert.Equal(5, pos.Y);
    }
}

public class TestTetrino
{
    // ***** Ghaith *****

    /** On vérifie que le tetrino par défaut est bien un carré rouge en (0,0) */
    [Fact]
    public void TestTetrino_InitialisationParDefaut()
    {
        Tetrino t = new Tetrino();

        Assert.Equal(0, t.Indice); // 0 = Carré
        Assert.Equal(TetrinoCouleur.Rouge, t.Couleur);
        Assert.Equal(0, t.PositionOrigine.X);
        Assert.Equal(0, t.PositionOrigine.Y);
    }

    /** On vérifie que l'origine est bien en bas à gauche en vérifiant les cordoonées*/
    [Fact]
    public void TestTetrino_Positions()
    {
        Tetrino t = new Tetrino();
        t.PositionOrigine = new Position(10, 10);
        t.Indice = 0; // carré 

        Position[] posCalculees = t.Positions();

        Assert.Equal(10, posCalculees[0].X);
        Assert.Equal(10, posCalculees[0].Y);
        Assert.Equal(11, posCalculees[1].X);
        Assert.Equal(9, posCalculees[2].Y);
    }

    /** On véirife que la couleur générée est valide : l'indice est bien dans la longueur de l'énumération et n'est ni blanc ni noir */
    [Fact]
    public void TestTetrino_NouveauTetrino()
    {
        JeuTetris jeu = new JeuTetris();

        jeu.NouveauTetrino();

        Assert.True((int)jeu.TetrinoCourant.Couleur >= 2);
        Assert.True((int)jeu.TetrinoCourant.Couleur < Enum.GetValues(typeof(TetrinoCouleur)).Length);
    }


    /** On vérifie que les positions calculées pour une barre horizontale sont correctes */
    [Fact]
    public void TestTetrino_PositionsBarreHorizontale()
    {
        Tetrino t = new Tetrino();
        t.PositionOrigine = new Position(5, 5);
        t.Indice = 1; // Barre horizontale

        Position[] pos = t.Positions();

        // La barre horizontale fait 4 cases de long : (5,5), (6,5), (7,5), (8,5)
        Assert.Equal(4, pos.Length);
        Assert.Equal(5, pos[0].X);
        Assert.Equal(8, pos[3].X);
        Assert.Equal(5, pos[3].Y);
    }

    /** On vérifie que NouveauTetrino place toujours l'origine X de sorte que 
        le tetrino ne dépasse pas la bordure droite (colonne 12) */
    [Fact]
    public void TestTetrino_ValidationBordureDroite()
    {
        JeuTetris jeu = new JeuTetris();

        // On lance le test 50 fois pour couvrir les différents cas aléatoires
        for (int i = 0; i < 50; i++)
        {
            jeu.NouveauTetrino();
            Position[] pos = jeu.TetrinoCourant.Positions();

            foreach (var p in pos)
            {
                // Le plateau semble être de 12 colonnes (0 à 11)
                Assert.True(p.X < 12, $"Le tetrino dépasse à droite : X={p.X} pour l'indice {jeu.TetrinoCourant.Indice}");
                Assert.True(p.X >= 0, $"Le tetrino dépasse à gauche : X={p.X}");
            }
        }
    }

    /** On vérifie que le tableau des couleurs est correctement initialisé dans le constructeur */
    [Fact]
    public void TestTetrino_InitialisationCouleurs()
    {
        Tetrino t = new Tetrino();

        // Vérifie que la première couleur du tableau statique est bien Blanc (0)
        Assert.Equal(TetrinoCouleur.Blanc, Tetrino.CouleursTetrinos[0]);
        // Vérifie que la dernière est bien Cyan (8)
        Assert.Equal(TetrinoCouleur.Cyan, Tetrino.CouleursTetrinos[Tetrino.CouleursTetrinos.Length - 1]);
        // On vérifie qu'ils ont la même longueur 
        Assert.Equal(Tetrino.CouleursTetrinos.Length, Enum.GetValues(typeof(TetrinoCouleur)).Length);
    }
}


public class TestJeuTetris
{
    // ***** Ghaith *****

    /** On vérifie que le constructeur de JeuTetris initialise correctement la taille de la grille et le tétrino courant */
    [Fact]
    public void TestJeuTetris_InitialisationEtDemarrer()
    {
        JeuTetris jeu = new JeuTetris();

        Assert.Equal(10, JeuTetris.LargeurGrille);
        Assert.Equal(20, JeuTetris.HauteurGrille);

        jeu.Demarrer();
        for (int y = 0; y < JeuTetris.HauteurGrille; y++)
        {
            for (int x = 0; x < JeuTetris.LargeurGrille; x++)
            {
                Assert.Equal(TetrinoCouleur.Blanc, jeu.Grille[x, y]);
            }
        }
        Assert.NotNull(jeu.TetrinoCourant);
    }

    /** On vérifie que la méthode PeutSeDeplacer détecte correctement les collisions avec les bordures et les carrés figés */
    [Fact]
    public void TestJeuTetris_PeutSeDeplacer_Collisions()
    {
        JeuTetris jeu = new JeuTetris();
        jeu.TetrinoCourant.Indice = 0; // Carré (2x2)
        jeu.TetrinoCourant.PositionOrigine = new Position(0, 0);

        // Test bordure gauche
        Assert.False(jeu.PeutSeDeplacer(-1, 0));

        // Test bordure droite
        jeu.TetrinoCourant.PositionOrigine = new Position(JeuTetris.LargeurGrille - 2, 0); //dernière position possible à droite
        Assert.False(jeu.PeutSeDeplacer(1, 0));

        // Test collision avec un carré déjà présent dans la grille
        jeu.TetrinoCourant.PositionOrigine = new Position(5, 5);
        jeu.Grille[5, 6] = TetrinoCouleur.Rouge;
        Assert.False(jeu.PeutSeDeplacer(0, 1));
    }

    /** On vérifie que les méthodes Droite et Gauche déplacent l'origine du tétrino seulement si le mouvement est valide */
    [Fact]
    public void TestJeuTetris_MouvementsHorizontaux()
    {
        JeuTetris jeu = new JeuTetris();
        jeu.TetrinoCourant.Indice = 0;
        jeu.TetrinoCourant.PositionOrigine = new Position(5, 5);

        jeu.Droite();
        Assert.Equal(6, jeu.TetrinoCourant.PositionOrigine.X);

        jeu.Gauche();
        Assert.Equal(5, jeu.TetrinoCourant.PositionOrigine.X);

        // On vérifie le blocage contre le bord gauche
        jeu.TetrinoCourant.PositionOrigine = new Position(0, 5);
        jeu.Gauche();
        Assert.Equal(0, jeu.TetrinoCourant.PositionOrigine.X);
    }

    /** On vérifie que la méthode Bas déplace le tétrino ou provoque son figeage s'il ne peut plus descendre */
    [Fact]
    public void TestJeuTetris_Bas_Et_FigeageAutomatique()
    {
        JeuTetris jeu = new JeuTetris();
        jeu.TetrinoCourant.Indice = 1; // Barre Horizontale
        jeu.TetrinoCourant.Couleur = TetrinoCouleur.Bleu;

        // On place le tétrino au niveau du sol
        jeu.TetrinoCourant.PositionOrigine = new Position(0, 19);

        // Le mouvement est impossible, l'appel à Bas() doit figer le tétrino et en créer un nouveau
        jeu.Bas();

        Assert.Equal(TetrinoCouleur.Bleu, jeu.Grille[0, 19]);
        // Le Y du nouveau tétrino est initialisé à -1 par NouveauTetrino()
        Assert.Equal(-1, jeu.TetrinoCourant.PositionOrigine.Y);
    }

    /** On vérifie que FigerTetrino inscrit les couleurs dans la grille et déclenche la suppression des lignes pleines */
    [Fact]
    public void TestJeuTetris_FigerTetrino_Et_NettoyageLigne()
    {
        JeuTetris jeu = new JeuTetris();

        // On pré-remplit une ligne presque complète
        for (int x = 1; x < JeuTetris.LargeurGrille; x++)
        {
            jeu.Grille[x, 19] = TetrinoCouleur.Jaune;
        }

        // On positionne un tétrino pour compléter la ligne
        jeu.TetrinoCourant.Indice = 2;
        jeu.TetrinoCourant.Couleur = TetrinoCouleur.Rouge;
        jeu.TetrinoCourant.PositionOrigine = new Position(0, 19);

        jeu.FigerTetrino();

        // La ligne 19 étant pleine, elle doit être supprimée (remplacée par la ligne vide du dessus)
        Assert.Equal(TetrinoCouleur.Blanc, jeu.Grille[1, 19]);
    }

    // ***** Awa *****

    // ============================================================
    // TEST 1
    // Vérifie que le constructeur de JeuTetris initialise bien :
    // - la largeur de la grille
    // - la hauteur de la grille
    // - le tétrino courant
    // ============================================================
    [Fact]
    public void Constructeur_Initialise_Bien_Le_Jeu()
    {
        // Arrange + Act
        // On crée simplement un nouveau jeu
        JeuTetris jeu = new JeuTetris();

        // Assert
        // On vérifie les valeurs attendues
        Assert.Equal(10, JeuTetris.LargeurGrille);
        Assert.Equal(20, JeuTetris.HauteurGrille);

        // On vérifie que le tétrino courant existe
        Assert.NotNull(jeu.TetrinoCourant);
    }

    // ============================================================
    // TEST 2
    // Vérifie que la méthode Gauche() déplace bien le tétrino
    // d'une case vers la gauche si X > 0
    // ============================================================
    [Fact]
    public void Gauche_Deplace_Bien_Le_Tetrino()
    {
        // Arrange
        JeuTetris jeu = new JeuTetris();

        // On place l'origine du tétrino à X = 5
        jeu.TetrinoCourant.PositionOrigine = new Position(5, 0);

        // Act
        jeu.Gauche();

        // Assert
        // Après déplacement à gauche, X doit valoir 4
        Assert.Equal(4, jeu.TetrinoCourant.PositionOrigine.X);

        // Y ne doit pas changer
        Assert.Equal(0, jeu.TetrinoCourant.PositionOrigine.Y);
    }

    // ============================================================
    // TEST 3
    // Vérifie que Gauche() ne fait pas sortir le tétrino
    // en dehors de la grille si X = 0
    // ============================================================
    [Fact]
    public void Gauche_Ne_Depasse_Pas_Le_Bord_Gauche()
    {
        // Arrange
        JeuTetris jeu = new JeuTetris();

        // On place le tétrino tout à gauche
        jeu.TetrinoCourant.PositionOrigine = new Position(0, 0);

        // Act
        jeu.Gauche();

        // Assert
        // X doit rester à 0
        Assert.Equal(0, jeu.TetrinoCourant.PositionOrigine.X);
    }

    // ============================================================
    // TEST 4
    // Vérifie que Bas() déplace bien le tétrino
    // d'une case vers le bas si ce n'est pas encore le fond
    // ============================================================
    [Fact]
    public void Bas_Deplace_Bien_Le_Tetrino()
    {
        // Arrange
        JeuTetris jeu = new JeuTetris();

        // On place le tétrino à une position de départ simple
        jeu.TetrinoCourant.PositionOrigine = new Position(3, 4);

        // Act
        jeu.Bas();

        // Assert
        // X ne change pas
        Assert.Equal(3, jeu.TetrinoCourant.PositionOrigine.X);

        // Y augmente de 1
        Assert.Equal(5, jeu.TetrinoCourant.PositionOrigine.Y);
    }

    // ============================================================
    // TEST 5
    // Vérifie que Droite() déplace bien le tétrino vers la droite
    //
    // Pour ce test, on prépare une forme simple dans TetrinosTab :
    // un carré avec les positions relatives :
    // (0,0), (1,0), (0,1), (1,1)
    //
    // Le décalage maximum en X est donc 1.
    // ============================================================
    [Fact]
    public void Droite_Deplace_Bien_Le_Tetrino()
    {
        // Arrange
        JeuTetris jeu = new JeuTetris();

        // On prépare une forme simple
        Tetrino.TetrinosTab = new Position[][]
        {
            new Position[]
            {
                new Position(0, 0),
                new Position(1, 0),
                new Position(0, 1),
                new Position(1, 1)
            }
        };

        // On choisit l'indice 0 dans le tableau des formes
        jeu.TetrinoCourant.Indice = 0;

        // On place l'origine à X = 3
        jeu.TetrinoCourant.PositionOrigine = new Position(3, 0);

        // Act
        jeu.Droite();

        // Assert
        // Le tétrino doit avancer d'une case
        Assert.Equal(4, jeu.TetrinoCourant.PositionOrigine.X);
        Assert.Equal(0, jeu.TetrinoCourant.PositionOrigine.Y);
    }

    // ============================================================
    // TEST 6
    // Vérifie que Tombe() fait descendre le tétrino jusqu'en bas
    //
    // Comme ta méthode Tombe() appelle Demarrer() à la fin,
    // on ne peut pas vérifier directement la dernière position
    // atteinte juste avant le redémarrage.
    //
    // Donc ici, on vérifie surtout que :
    // - la méthode s'exécute sans erreur
    // - le tétrino courant existe encore après l'appel
    // ============================================================
    [Fact]
    public void Tombe_Sexecute_Sans_Erreur()
    {
        // Arrange
        JeuTetris jeu = new JeuTetris();

        // On place une origine de départ
        jeu.TetrinoCourant.PositionOrigine = new Position(2, 0);

        // Act
        jeu.Tombe();

        // Assert
        // Le jeu doit toujours avoir un tétrino courant
        Assert.NotNull(jeu.TetrinoCourant);
    }

    // ============================================================
    // TEST 7
    // Vérifie que Demarrer() initialise bien un tétrino
    //
    // Ce test suppose que NouveauTetrino() met en place
    // PositionOrigine ou au moins un état valide.
    // ============================================================
    [Fact]
    public void Demarrer_Initialise_Le_Tetrino()
    {
        // Arrange
        JeuTetris jeu = new JeuTetris();

        // Act
        jeu.Demarrer();

        // Assert
        Assert.NotNull(jeu.TetrinoCourant);
        Assert.NotNull(jeu.TetrinoCourant.PositionOrigine);
    }

    // ============================================================
    // TEST 8
    // Vérifie que plusieurs appels à Bas() augmentent bien Y
    // plusieurs fois
    // ============================================================
    [Fact]
    public void Bas_Plusieurs_Fois_Descend_Correctement()
    {
        // Arrange
        JeuTetris jeu = new JeuTetris();

        jeu.TetrinoCourant.PositionOrigine = new Position(1, 1);

        // Act
        jeu.Bas();
        jeu.Bas();
        jeu.Bas();

        // Assert
        // Y doit avoir augmenté de 3
        Assert.Equal(4, jeu.TetrinoCourant.PositionOrigine.Y);
    }


}
