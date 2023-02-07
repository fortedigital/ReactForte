namespace ReactForte.Application.Webpack;

public class WebpackOptions
{
    public string OutputPath { get; set; } = null!;

    public bool IsDevServer => OutputPath.Contains("localhost");
}