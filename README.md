# ExpressZen

ExpressZen is a highly differentiated Regular Expression testing and building tool built with Blazor WebAssembly (WASM). It helps users build, test, and understand regular expressions using a neuro-inclusive "Zen Mode" UI, all executing locally in the browser with .NET 8.

## Features

- **TDD Regex Workspace**: Instantly test patterns against 'Must Match' and 'Must NOT Match' data.
- **AI Integrations**: Native "Bring Your Own Key" (BYOK) support for Google Gemini, OpenAI, and Anthropic.
  - **Regex Builder**: Describe regex in plain English and let AI build it.
  - **Regex Explainer**: Get a neuro-inclusive, plain English explanation of any regex pattern.
  - **Infer Test Data**: Automatically generate valid and invalid test strings based on your pattern.
- **Escaping Hell Translator**: Automatically translates patterns into valid C# strings (verbatim, raw, etc).
- **Interactive Reference**: Quick cheat sheet for regex tokens and anchors.

## Getting Started

ExpressZen is a purely client-side application. 

```bash
cd ExpressZen.Client
dotnet run
```

Navigate to the provided localhost URL and input your preferred AI provider's API key to unlock the intelligent features! No backend or database required.
