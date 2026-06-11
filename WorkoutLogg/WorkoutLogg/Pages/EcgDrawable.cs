namespace WorkoutLogg.Pages;

public class EcgDrawable : IDrawable
{
    // ECG keypoints normalized to [0,1] in a 200×70 viewbox
    private static readonly (float Nx, float Ny)[] s_pts =
    [
        (0f / 200f,   40f / 70f),
        (44f / 200f,  40f / 70f),
        (58f / 200f,  22f / 70f),
        (76f / 200f,  56f / 70f),
        (94f / 200f,  14f / 70f),
        (108f / 200f, 40f / 70f),
        (200f / 200f, 40f / 70f),
    ];

    public float DrawProgress { get; set; } = 0f;  // 0..1
    public float Opacity { get; set; } = 1f;        // 0..1

    public void Draw(ICanvas canvas, RectF d)
    {
        if (Opacity <= 0.01f) return;

        float w = d.Width;
        float h = d.Height;

        float[] px = [.. s_pts.Select(p => p.Nx * w)];
        float[] py = [.. s_pts.Select(p => p.Ny * h)];

        float[] lens = new float[s_pts.Length - 1];
        float total = 0f;
        for (int i = 0; i < lens.Length; i++)
        {
            float dx = px[i + 1] - px[i];
            float dy = py[i + 1] - py[i];
            lens[i] = MathF.Sqrt(dx * dx + dy * dy);
            total += lens[i];
        }

        float target = DrawProgress * total;
        float consumed = 0f;

        canvas.StrokeSize = 5f;
        canvas.StrokeLineCap = LineCap.Round;

        for (int i = 0; i < lens.Length; i++)
        {
            float left = target - consumed;
            if (left <= 0f) break;

            float ax = px[i], ay = py[i];
            float bx, by;

            if (left >= lens[i])
            {
                bx = px[i + 1]; by = py[i + 1];
                consumed += lens[i];
            }
            else
            {
                float t = left / lens[i];
                bx = ax + t * (px[i + 1] - ax);
                by = ay + t * (py[i + 1] - ay);
                consumed = target;
            }

            // gradient purple → pink based on horizontal midpoint
            float mid = (ax + bx) / 2f / w;
            canvas.StrokeColor = Gradient(mid, Opacity);
            canvas.DrawLine(ax, ay, bx, by);
        }
    }

    private static Color Gradient(float t, float alpha)
    {
        float r = 0x7C / 255f + t * (0xEC / 255f - 0x7C / 255f);
        float g = 0x3A / 255f + t * (0x48 / 255f - 0x3A / 255f);
        float b = 0xED / 255f + t * (0x99 / 255f - 0xED / 255f);
        return new Color(r, g, b, alpha);
    }
}
