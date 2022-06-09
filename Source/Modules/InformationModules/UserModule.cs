using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using PenileNET.Utilities;
using PenileNET.Utilities.Constants;

namespace PenileNET.Modules {
    [Group("user", "Commands for getting and manipulating users.")]
    public class UserModule : InteractionModuleBase<SocketInteractionContext> {
        public InteractionService? Commands { get; set; }

        [SlashCommand("display", "Displays the user's information.")]
        public async Task Display(
            SocketGuildUser? user = null,
            [Choice("user", 0)] [Choice("guild", 1)] [Choice("all", 2)]
            int? option = null
        ) {
            user = UserTools.GetUser(user, Context);

            switch (option) {
            case 0:
                await RespondAsync(
                    embed: UserTools.UserProfileEmbed(user).Build()
                );

                break;
            case 1:
                await RespondAsync(
                    embed: UserTools.GuildProfileEmbed(user).Build()
                );

                break;
            case 2:
                await RespondAsync(
                    embed: UserTools.AllProfileEmbed(user).Build()
                );

                break;
            default:
                await RespondAsync(
                    embed: UserTools.ProfileEmbed(user).Build()
                );

                break;
            }
        }


        [SlashCommand("roles", "Display or modify the user's roles.")]
        public async Task Roles(SocketGuildUser? user = null, SocketRole? role = null) {
            user = UserTools.GetUser(user, Context);
            var roles = GeneralTools.GetSorted(user.Roles.ToList());
            var embed = UserTools.ProfileEmbed(user);

            if (role == null) {
                embed.AddField(
                    new EmbedFieldBuilder {
                        Name = $"Roles [{roles.Count}]",
                        Value = UserTools.FormatRoles(roles)
                    }
                );
            } else {
                if (user.GuildPermissions.ManageRoles) {
                    return;
                }

                if (user.Roles.Contains(role)) {
                    try {
                        await user.RemoveRoleAsync(role);
                    } catch {
                        await RespondAsync(
                            embed: new EmbedBuilder {
                                Color = Colors.Offline,
                                Title = "There was an error removing the role from the user."
                            }.Build(),
                            ephemeral: true
                        );

                        return;
                    }

                    embed.AddField(
                        new EmbedFieldBuilder {
                            Name = "Role Removed",
                            Value = role.Mention
                        }
                    );
                } else {
                    try {
                        await user.AddRoleAsync(role);
                    } catch {
                        await RespondAsync(
                            embed: new EmbedBuilder {
                                Color = Colors.Offline,
                                Title = "There was an error adding the role to the user."
                            }.Build(),
                            ephemeral: true
                        );

                        return;
                    }

                    embed.AddField(
                        new EmbedFieldBuilder {
                            Name = "Role Added",
                            Value = role.Mention
                        }
                    );
                }
            }

            await RespondAsync(
                embed: embed.Build()
            );
        }

        [SlashCommand("nick", "Display or set the user's nickname.")]
        public async Task Nick(SocketGuildUser? user = null, string? nickname = null) {
            user = UserTools.GetUser(user, Context);
            var embed = UserTools.ProfileEmbed(user);

            if (nickname == null) {
                var field = new EmbedFieldBuilder {
                    Name = "Nickname"
                };

                if (string.IsNullOrWhiteSpace(user.Nickname)) {
                    field.Value = "`Undefined`";
                } else {
                    field.Value = user.Mention;
                }

                embed.AddField(field);
            } else {
                if (user.GuildPermissions.ManageNicknames) {
                    return;
                }

                try {
                    await user.ModifyAsync(x =>
                        x.Nickname = nickname
                    );
                } catch {
                    await RespondAsync(
                        embed: new EmbedBuilder {
                            Color = Colors.Offline,
                            Title = "There was an error setting the user's nickname."
                        }.Build(),
                        ephemeral: true
                    );

                    return;
                }

                embed.AddField(
                    new EmbedFieldBuilder {
                        Name = "Nickname Set",
                        Value = user.Mention
                    }
                );
            }

            await RespondAsync(
                embed: embed.Build()
            );
        }

        [SlashCommand("mute", "If the user is muted, un-mute. Otherwise, mute.")]
        public async Task Mute(SocketGuildUser? user = null) {
            user = UserTools.GetUser(user, Context);
            var embed = UserTools.ProfileEmbed(user);

            if (user.IsMuted) {
                try {
                    await user.ModifyAsync(x =>
                        x.Mute = false
                    );
                } catch {
                    await RespondAsync(
                        embed: new EmbedBuilder {
                            Color = Colors.Offline,
                            Title = "There was an error un-muting the user."
                        }.Build(),
                        ephemeral: true
                    );

                    return;
                }

                embed.AddField(
                    new EmbedFieldBuilder {
                        Name = "Un-muted",
                        Value = user.Mention
                    }
                );
            } else {
                if (user.GuildPermissions.MuteMembers) {
                    return;
                }

                try {
                    await user.ModifyAsync(x =>
                        x.Mute = true
                    );
                } catch {
                    await RespondAsync(
                        embed: new EmbedBuilder {
                            Color = Colors.Offline,
                            Title = "There was an error muting the user."
                        }.Build(),
                        ephemeral: true
                    );

                    return;
                }

                embed.AddField(
                    new EmbedFieldBuilder {
                        Name = "Muted",
                        Value = user.Mention
                    }
                );
            }

            await RespondAsync(
                embed: embed.Build()
            );
        }

        [SlashCommand("deafen", "If the user is deafened, un-deafen. Otherwise, deafen.")]
        public async Task Deafen(SocketGuildUser? user = null) {
            user = UserTools.GetUser(user, Context);
            var embed = UserTools.ProfileEmbed(user);

            if (user.IsDeafened) {
                try {
                    await user.ModifyAsync(x =>
                        x.Deaf = false
                    );
                } catch {
                    await RespondAsync(
                        embed: new EmbedBuilder {
                            Color = Colors.Offline,
                            Title = "There was an error un-deafening the user."
                        }.Build(),
                        ephemeral: true
                    );

                    return;
                }

                embed.AddField(
                    new EmbedFieldBuilder {
                        Name = "Un-deafened",
                        Value = user.Mention
                    }
                );
            } else {
                if (user.GuildPermissions.DeafenMembers) {
                    return;
                }

                try {
                    await user.ModifyAsync(x =>
                        x.Deaf = false
                    );
                } catch {
                    await RespondAsync(
                        embed: new EmbedBuilder {
                            Color = Colors.Offline,
                            Title = "There was an error deafening the user."
                        }.Build(),
                        ephemeral: true
                    );

                    return;
                }

                embed.AddField(
                    new EmbedFieldBuilder {
                        Name = "Deafened",
                        Value = user.Mention
                    }
                );
            }

            await RespondAsync(
                embed: embed.Build()
            );
        }

        [DefaultMemberPermissions(GuildPermission.KickMembers)]
        [SlashCommand("kick", "Kicks the user.")]
        public async Task KicK(SocketGuildUser user, string reason = "There is no reason.") {
            user = UserTools.GetUser(user, Context);

            try {
                await user.KickAsync(reason);
            } catch {
                await RespondAsync(
                    embed: new EmbedBuilder {
                        Color = Colors.Offline,
                        Title = "There was an error kicking the user."
                    }.Build(),
                    ephemeral: true
                );

                return;
            }

            await RespondAsync(
                embed: UserTools.ProfileEmbed(user)
                    .AddField(
                        new EmbedFieldBuilder {
                            IsInline = true,
                            Name = "Kicked",
                            Value = $"```{reason}```"
                        }
                    ).Build()
            );
        }

        [DefaultMemberPermissions(GuildPermission.BanMembers)]
        [SlashCommand("ban", "Bans the user.")]
        public async Task Ban(
            SocketGuildUser user,
            [MinValue(0)] [MaxValue(7)] int days = 0,
            string reason = "There is no reason."
        ) {
            user = UserTools.GetUser(user, Context);

            try {
                await user.BanAsync(days, reason);
            } catch {
                await RespondAsync(
                    embed: new EmbedBuilder {
                        Color = Colors.Offline,
                        Title = "There was an error banning the user."
                    }.Build(),
                    ephemeral: true
                );

                return;
            }

            await RespondAsync(
                embed: UserTools.ProfileEmbed(user)
                    .AddField(
                        new EmbedFieldBuilder {
                            IsInline = true,
                            Name = "Banned",
                            Value = $"```{reason}```"
                        }
                    ).Build()
            );
        }
    }
}