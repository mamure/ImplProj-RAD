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
        for (int i = 0; i < m; i++){
            sketch[i] = 0;
        }
    }

    public ulong CalculateEstimate(){
        ulong X = 0;
        for (int y = 0; y < m; y++){
            X += (ulong)(sketch[y]*sketch[y]);
        }
        return X;
    }

    public void Process(IEnumerable<(ulong, int)> stream){
        foreach (var (x,d) in stream){
            sketch[h(x)] += s(x)*d;
        }
    }


    
}
