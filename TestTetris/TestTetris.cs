using NoyauTetris;

namespace TestTetris;

public class TestNoyeau
{
    /** On vérifie que le déplacement se fait sans erreur */
    [Fact]
    public void TestPosition_DeplacementBas()
    {
        Position pos = new Position(5, 5);

        pos.DeplacerBas();

        Assert.Equal(6, pos.Y);
        Assert.Equal(5, pos.X);
    }

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
        Tetrino t = new Tetrino();

        t.NouveauTetrino();

        Assert.True((int)t.Couleur >= 2);
        Assert.True((int)t.Couleur < Enum.GetValues(typeof(TetrinoCouleur)).Length);
    }
}
