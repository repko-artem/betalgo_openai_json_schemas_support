using System.Text.Json;
using Betalgo.Ranul.OpenAI.ObjectModels.ResponseModels;

namespace OpenAI.Utilities.Tests;

public class NullableTokenDetailsDeserializationTests
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    #region CompletionTokensDetails

    [Fact]
    public void CompletionTokensDetails_WithNullAudioTokens_DeserializesWithoutException()
    {
        const string json = """{"audio_tokens": null, "reasoning_tokens": null}""";

        var result = JsonSerializer.Deserialize<CompletionTokensDetails>(json, JsonOptions);

        result.ShouldNotBeNull();
        result.AudioTokens.ShouldBeNull();
        result.ReasoningTokens.ShouldBeNull();
    }

    [Fact]
    public void CompletionTokensDetails_WithValidValues_DeserializesCorrectly()
    {
        const string json = """{"audio_tokens": 42, "reasoning_tokens": 100, "text_tokens": 65}""";

        var result = JsonSerializer.Deserialize<CompletionTokensDetails>(json, JsonOptions);

        result.ShouldNotBeNull();
        result.AudioTokens.ShouldBe(42);
        result.ReasoningTokens.ShouldBe(100);
        result.TextTokens.ShouldBe(65);
    }

    [Fact]
    public void CompletionTokensDetails_WithMissingFields_DefaultsToNull()
    {
        const string json = """{}""";

        var result = JsonSerializer.Deserialize<CompletionTokensDetails>(json, JsonOptions);

        result.ShouldNotBeNull();
        result.AudioTokens.ShouldBeNull();
        result.ReasoningTokens.ShouldBeNull();
        result.TextTokens.ShouldBeNull();
    }

    [Fact]
    public void CompletionTokensDetails_WithZeroValues_DeserializesCorrectly()
    {
        const string json = """{"audio_tokens": 0, "reasoning_tokens": 0, "text_tokens": 65}""";

        var result = JsonSerializer.Deserialize<CompletionTokensDetails>(json, JsonOptions);

        result.ShouldNotBeNull();
        result.AudioTokens.ShouldBe(0);
        result.ReasoningTokens.ShouldBe(0);
        result.TextTokens.ShouldBe(65);
    }

    #endregion

    #region PromptTokensDetails

    [Fact]
    public void PromptTokensDetails_WithAllNullFields_DeserializesWithoutException()
    {
        const string json = """
            {
                "cached_tokens": 0,
                "audio_tokens": null,
                "text_tokens": null,
                "image_tokens": null,
                "video_tokens": null,
                "cache_creation_tokens": null
            }
            """;

        var result = JsonSerializer.Deserialize<PromptTokensDetails>(json, JsonOptions);

        result.ShouldNotBeNull();
        result.CachedTokens.ShouldBe(0);
        result.AudioTokens.ShouldBeNull();
        result.TextTokens.ShouldBeNull();
        result.ImageTokens.ShouldBeNull();
        result.VideoTokens.ShouldBeNull();
        result.CacheCreationTokens.ShouldBeNull();
    }

    [Fact]
    public void PromptTokensDetails_WithValidValues_DeserializesCorrectly()
    {
        const string json = """
            {
                "cached_tokens": 10,
                "audio_tokens": 5,
                "text_tokens": 3495,
                "image_tokens": 0,
                "video_tokens": 0,
                "cache_creation_tokens": 0
            }
            """;

        var result = JsonSerializer.Deserialize<PromptTokensDetails>(json, JsonOptions);

        result.ShouldNotBeNull();
        result.CachedTokens.ShouldBe(10);
        result.AudioTokens.ShouldBe(5);
        result.TextTokens.ShouldBe(3495);
        result.ImageTokens.ShouldBe(0);
        result.VideoTokens.ShouldBe(0);
        result.CacheCreationTokens.ShouldBe(0);
    }

    [Fact]
    public void PromptTokensDetails_WithOnlyCachedTokens_BackwardCompatible()
    {
        const string json = """{"cached_tokens": 12}""";

        var result = JsonSerializer.Deserialize<PromptTokensDetails>(json, JsonOptions);

        result.ShouldNotBeNull();
        result.CachedTokens.ShouldBe(12);
        result.AudioTokens.ShouldBeNull();
        result.TextTokens.ShouldBeNull();
    }

    #endregion

    #region UsageResponse

    [Fact]
    public void UsageResponse_WithNullCompletionTokensDetails_DeserializesCorrectly()
    {
        const string json = """
            {
                "prompt_tokens": 100,
                "completion_tokens": 50,
                "total_tokens": 150,
                "completion_tokens_details": null
            }
            """;

        var result = JsonSerializer.Deserialize<UsageResponse>(json, JsonOptions);

        result.ShouldNotBeNull();
        result.PromptTokens.ShouldBe(100);
        result.CompletionTokens.ShouldBe(50);
        result.TotalTokens.ShouldBe(150);
        result.CompletionTokensDetails.ShouldBeNull();
    }

    [Fact]
    public void UsageResponse_WithUnknownFields_IgnoresThemSuccessfully()
    {
        const string json = """
            {
                "prompt_tokens": 3495,
                "completion_tokens": 65,
                "total_tokens": 3560,
                "cache_read_input_tokens": 0,
                "cache_creation_input_tokens": 0,
                "unknown_future_field": "some_value"
            }
            """;

        var result = JsonSerializer.Deserialize<UsageResponse>(json, JsonOptions);

        result.ShouldNotBeNull();
        result.PromptTokens.ShouldBe(3495);
        result.CompletionTokens.ShouldBe(65);
        result.TotalTokens.ShouldBe(3560);
    }

    /// <summary>
    /// Regression test for SR-01492852 (Ecommpay).
    /// LiteLLM v1.82.x returns null for token detail fields causing deserialization crash.
    /// </summary>
    [Fact]
    public void UsageResponse_WithRealLiteLlm182xResponse_DeserializesWithoutException()
    {
        const string json = """
            {
                "total_tokens": 3560,
                "prompt_tokens": 3495,
                "completion_tokens": 65,
                "prompt_tokens_details": {
                    "text_tokens": null,
                    "audio_tokens": null,
                    "image_tokens": null,
                    "video_tokens": null,
                    "cached_tokens": 0,
                    "cache_creation_tokens": 0
                },
                "cache_read_input_tokens": 0,
                "completion_tokens_details": {
                    "text_tokens": 65,
                    "reasoning_tokens": 0
                },
                "cache_creation_input_tokens": 0
            }
            """;

        var result = JsonSerializer.Deserialize<UsageResponse>(json, JsonOptions);

        result.ShouldNotBeNull();
        result.PromptTokens.ShouldBe(3495);
        result.CompletionTokens.ShouldBe(65);
        result.TotalTokens.ShouldBe(3560);
        result.PromptTokensDetails.ShouldNotBeNull();
        result.PromptTokensDetails!.CachedTokens.ShouldBe(0);
        result.PromptTokensDetails.AudioTokens.ShouldBeNull();
        result.PromptTokensDetails.TextTokens.ShouldBeNull();
        result.PromptTokensDetails.ImageTokens.ShouldBeNull();
        result.PromptTokensDetails.VideoTokens.ShouldBeNull();
        result.CompletionTokensDetails.ShouldNotBeNull();
        result.CompletionTokensDetails!.TextTokens.ShouldBe(65);
        result.CompletionTokensDetails.ReasoningTokens.ShouldBe(0);
    }

    /// <summary>
    /// Regression test for SR-01492852: agent skill invocation response with null token details.
    /// </summary>
    [Fact]
    public void UsageResponse_WithNullCompletionTokensDetailsAndNullPromptDetailFields_DeserializesWithoutException()
    {
        const string json = """
            {
                "total_tokens": 12323,
                "prompt_tokens": 12283,
                "completion_tokens": 40,
                "prompt_tokens_details": {
                    "text_tokens": null,
                    "audio_tokens": null,
                    "image_tokens": null,
                    "cached_tokens": 0,
                    "cache_creation_tokens": 0
                },
                "completion_tokens_details": null,
                "cache_creation_input_tokens": 0
            }
            """;

        var result = JsonSerializer.Deserialize<UsageResponse>(json, JsonOptions);

        result.ShouldNotBeNull();
        result.PromptTokens.ShouldBe(12283);
        result.CompletionTokens.ShouldBe(40);
        result.CompletionTokensDetails.ShouldBeNull();
        result.PromptTokensDetails.ShouldNotBeNull();
        result.PromptTokensDetails!.AudioTokens.ShouldBeNull();
    }

    #endregion

    #region Full ChatCompletionCreateResponse

    /// <summary>
    /// End-to-end regression test: full ChatCompletionCreateResponse deserialization
    /// with real LiteLLM v1.82.x JSON from case SR-01492852 (Ecommpay).
    /// </summary>
    [Fact]
    public void ChatCompletionCreateResponse_WithRealLiteLlm182xJson_DeserializesWithoutException()
    {
        const string json = """
            {
                "id": "chatcmpl-beb7f49a-bf6f-482c-b61f-d70c3805f7f4",
                "model": "litellm.eu.anthropic.claude-sonnet-4-6",
                "object": "chat.completion",
                "choices": [
                    {
                        "index": 0,
                        "message": {
                            "role": "assistant",
                            "content": "Hello! Welcome to the Creatio AI Assistant!",
                            "tool_calls": null,
                            "function_call": null
                        },
                        "finish_reason": "stop"
                    }
                ],
                "usage": {
                    "total_tokens": 3560,
                    "prompt_tokens": 3495,
                    "completion_tokens": 65,
                    "prompt_tokens_details": {
                        "text_tokens": null,
                        "audio_tokens": null,
                        "image_tokens": null,
                        "video_tokens": null,
                        "cached_tokens": 0,
                        "cache_creation_tokens": 0
                    },
                    "cache_read_input_tokens": 0,
                    "completion_tokens_details": {
                        "text_tokens": 65,
                        "reasoning_tokens": 0
                    },
                    "cache_creation_input_tokens": 0
                },
                "created": 1774515205,
                "system_fingerprint": null
            }
            """;

        var result = JsonSerializer.Deserialize<ChatCompletionCreateResponse>(json, JsonOptions);

        result.ShouldNotBeNull();
        result.Id.ShouldBe("chatcmpl-beb7f49a-bf6f-482c-b61f-d70c3805f7f4");
        result.Model.ShouldBe("litellm.eu.anthropic.claude-sonnet-4-6");
        result.Choices.ShouldNotBeNull();
        result.Choices!.Count.ShouldBe(1);
        result.Usage.ShouldNotBeNull();
        result.Usage!.PromptTokens.ShouldBe(3495);
        result.Usage.CompletionTokens.ShouldBe(65);
        result.Usage.PromptTokensDetails!.AudioTokens.ShouldBeNull();
        result.Usage.CompletionTokensDetails!.ReasoningTokens.ShouldBe(0);
    }

    #endregion
}
