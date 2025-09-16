var builder = DistributedApplication.CreateBuilder(args);

var postgresTag = "17-alpine";
var seqTag = "2025.2";
var flagdServerTag = "latest";

// postgres host for all the databases
var postgresHost = builder.AddPostgres("postgres")
                          .WithImageTag(postgresTag)
                          .WithDataBindMount(source: "../../containers/postgres/data", isReadOnly: false)
                          .WithPgAdmin(pgAdmin =>
                          {
                              pgAdmin.ExcludeFromManifest();
                              pgAdmin.WithLifetime(ContainerLifetime.Persistent);
                              pgAdmin.WithHostPort(6060);
                          })
                          .ExcludeFromManifest()
                          .WithLifetime(ContainerLifetime.Persistent);

// postgres db - url shortener
var urlShortenerDb = postgresHost.AddDatabase("urlshortenerdb");

// Seq containerized instance to support log review.
var seq = builder.AddSeq("SeqServer", port: 5341)
                 .WithImageTag(seqTag)
                 .WithDataBindMount("../../containers/seq/data")
                 .ExcludeFromManifest()
                 .WithLifetime(ContainerLifetime.Persistent)
                 .WithEnvironment("ACCEPT_EULA", "Y")
                 .WithEnvironment("SEQ_FIRSTRUN_NOAUTHENTICATION", "true");

// flagd containerized instance for feature flag support.
// AddContainer does not technically support "WithReference" so we need to do some fanagling in projects.
var flagd = builder.AddContainer("FlagdServer", "ghcr.io/open-feature/flagd", flagdServerTag)
                 .WithBindMount("../../containers/flagd/data", "/flags")
                 .WithArgs("start", "-f", "file:/flags/flagd.json")
                 .WithHttpEndpoint(8013, 8013)
                 .ExcludeFromManifest()
                 .WithLifetime(ContainerLifetime.Persistent);

builder.AddProject<Projects.Url_Shortener_Api>("url-shortener-api")
       .WithReference(urlShortenerDb)
       .WithReference(seq)
       .WithEnvironment("ConnectionStrings__FlagdServer", flagd.GetEndpoint("http"))
       .WaitFor(urlShortenerDb)
       .WaitFor(seq)
       .WaitFor(flagd);

builder.Build().Run();