public class CountSketch {
    private IHash h;
    private IHash s;
    private int m;
    private int[] sketch;
    public CountSketch(IHash h, IHash s) {
        this.h = h;
        this.s = s;
        m = 1 << h.BitLen();
        this.sketch = new int[m];
    }


    
}
