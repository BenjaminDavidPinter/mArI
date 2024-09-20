# mArI - The OpenAI C# Framework

## The Goal
mArI aims to be the most user friendly C# OpenAI framework, abstracting away as much as possible to expose simple functions which allow you to interact with GPT Assistants.

## How To Use

mArI is setup for dependency injection, and setup is accomplished through an extension method. Whereever your `ServiceCollection` is setup, simply call;

```csharp
services.AddMari();
```

You then only need to inject one service; ```OpenAiAssistantService```, which then allows you to interact with OpenAI Assistants.