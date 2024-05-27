public class CountSketch {
    private IHash h;
    private IHash s;
    private int m;
    public CountSketch(IHash h, IHash s) {
        this.h = h;
        this.s = s;
        m = h.BitLen();
    }
}