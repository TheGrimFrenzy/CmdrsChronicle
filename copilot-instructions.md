# Copilot Implementation Directives: CmdrsChronicle

This file documents key implementation rules and conventions for CmdrsChronicle, to be followed by all developers and automation (including Copilot) during code generation, schema mapping, and migrations.

## General Guidance for "Maybe" Prompts
- **Rule:** If the user uses the word "maybe" in a prompt, treat it as a request for input or recommendation before making any decision or change.
- **How to Apply:** Provide pros, cons, and a clear suggestion. Do not proceed with a change until the user confirms their preference.

## Ambiguity Handling
- **Rule:** If a user prompt or requirement is ambiguous, always surface the ambiguity and ask for clarification before proceeding. Never assume intent or fill in missing details without explicit user confirmation.
- **How to Apply:**
  - Clearly state what is ambiguous and why it matters for the next step.
  - Offer concrete options or ask targeted questions to resolve the ambiguity.
  - Wait for user input before making decisions or changes based on ambiguous information.
  - Document any resolved ambiguities for future reference if relevant.

## Explanation Before Action
- **Rule:** If the user says "show me", "tell me", or uses similar language, always explain your reasoning, plan, or approach before making any changes or taking action.
- **How to Apply:**
  - Provide a clear summary of your intended approach, options, or rationale.
  - Wait for user confirmation or feedback before proceeding with edits or execution.
  - Use this as an opportunity to surface tradeoffs, alternatives, or ask clarifying questions if needed.

---
Add new implementation directives here as they are defined in the project.
