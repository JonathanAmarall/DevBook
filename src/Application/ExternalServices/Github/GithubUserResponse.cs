using Refit;

namespace Application.ExternalServices.Github;

public record GithubUserResponse
{
    [AliasAs("login")]
    public string Login { get; init; }

    [AliasAs("id")]
    public int Id { get; init; }

    [AliasAs("node_id")]
    public string NodeId { get; init; }

    [AliasAs("avatar_url")]
    public Uri AvatarUrl { get; init; }

    [AliasAs("gravatar_id")]
    public string GravatarId { get; init; }

    [AliasAs("url")]
    public string Url { get; init; }

    [AliasAs("html_url")]
    public string HtmlUrl { get; init; }

    [AliasAs("followers_url")]
    public string FollowersUrl { get; init; }

    [AliasAs("following_url")]
    public string FollowingUrl { get; init; }

    [AliasAs("gists_url")]
    public string GistsUrl { get; init; }

    [AliasAs("starred_url")]
    public string StarredUrl { get; init; }

    [AliasAs("subscriptions_url")]
    public string SubscriptionsUrl { get; init; }

    [AliasAs("organizations_url")]
    public string OrganizationsUrl { get; init; }

    [AliasAs("repos_url")]
    public string ReposUrl { get; init; }

    [AliasAs("events_url")]
    public string EventsUrl { get; init; }

    [AliasAs("received_events_url")]
    public string ReceivedEventsUrl { get; init; }

    [AliasAs("type")]
    public string Type { get; init; }

    [AliasAs("user_view_type")]
    public string UserViewType { get; init; }

    [AliasAs("site_admin")]
    public bool SiteAdmin { get; init; }

    [AliasAs("name")]
    public string Name { get; init; }

    [AliasAs("company")]
    public string Company { get; init; }

    [AliasAs("blog")]
    public string Blog { get; init; }

    [AliasAs("location")]
    public string Location { get; init; }

    [AliasAs("email")]
    public string Email { get; init; }

    [AliasAs("hireable")]
    public object Hireable { get; init; }

    [AliasAs("bio")]
    public string Bio { get; init; }

    [AliasAs("twitter_username")]
    public object TwitterUsername { get; init; }

    [AliasAs("notification_email")]
    public string NotificationEmail { get; init; }

    [AliasAs("public_repos")]
    public int PublicRepos { get; init; }

    [AliasAs("public_gists")]
    public int PublicGists { get; init; }

    [AliasAs("followers")]
    public int Followers { get; init; }

    [AliasAs("following")]
    public int Following { get; init; }

    [AliasAs("created_at")]
    public DateTime CreatedAt { get; init; }

    [AliasAs("updated_at")]
    public DateTime UpdatedAt { get; init; }

    [AliasAs("private_gists")]
    public int PrivateGists { get; init; }

    [AliasAs("total_private_repos")]
    public int TotalPrivateRepos { get; init; }

    [AliasAs("owned_private_repos")]
    public int OwnedPrivateRepos { get; init; }

    [AliasAs("disk_usage")]
    public int DiskUsage { get; init; }

    [AliasAs("collaborators")]
    public int Collaborators { get; init; }

    [AliasAs("two_factor_authentication")]
    public bool TwoFactorAuthentication { get; init; }
}

