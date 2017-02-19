namespace Bundler.Infrastructure {
    public interface IContentBundler {
        string ContentType { get; }
        string GenerateTag(string src);

        string Placeholder { get; }

        IContentTransformer CreateTransformer();
    }
}
