# Plan: Core "Journey to Correctness" Script

## Overview
Create the primary two-presenter screenplay with "Yes, and..." conversational flow. This is the essential deliverable that must work before any add-ons are considered.

## Output
`presentation-script-journey.md` - 35-40 minute self-contained screenplay

## Steps

### Step 1: Create Script Structure
Create `presentation-script-journey.md` with:
- Header section (title, duration, presenter roles, physical staging notes)
- 4 act structure with timing markers
- Stage direction conventions (`[DEV A]`, `[DEV 1]`, `[TO AUDIENCE]`, `[TYPING]`, etc.)

### Step 2: Write Act 1 - Introduction & Problem Setup (Minutes 0-10)
**Scenes:**
- 1.1: Welcome & presenter introductions (0-2 min)
- 1.2: Audience poll and expectations (2-3 min)
- 1.3: Roadmap overview (3-5 min)
- 1.4: Business rule introduction (5-7 min)
- 1.5: Show naive C# model and its problems (7-10 min)

**Dialogue focus:** Establish conversational tone, introduce the problem together, first "Yes, and..." moment identifying illegal states.

**Embedded code:**
```csharp
class Customer {
  public string Id;
  public bool IsStandard;
  public bool IsRegistered;
}
```

### Step 3: Write Act 2 - First Improvements with DUs (Minutes 10-20)
**Scenes:**
- 2.1: F# type system quick intro (10-13 min)
- 2.2: Iteration 0 - naive F# matching naive C# (13-15 min)
- 2.3: Iteration 1 - introduce discriminated unions (15-18 min)
- 2.4: Show C# consuming the DU (18-20 min)

**Dialogue focus:** Dev A proposes separating customer types, Dev 1 responds "Yes, and we can use a discriminated union!" Show excitement at seeing illegal state become impossible.

**Embedded code:** Both F# type definitions and C# switch expressions at each iteration.

### Step 4: Write Act 3 - Making Tiers Explicit (Minutes 20-30)
**Scenes:**
- 3.1: Identify remaining boolean problem (20-21 min)
- 3.2: Iteration 2 - Standard as explicit case (21-25 min)
- 3.3: Run all test cases side-by-side (25-28 min)
- 3.4: Benefits discussion (28-30 min)

**Dialogue focus:** Dev 1 notices "IsStandard is still just a boolean flag...", Dev A responds "Yes, and we can make discount tiers a real domain concept!" Celebrate the simplified C# code.

**Embedded code:** Final F# Customer type and clean C# CalculateTotal switch expression.

### Step 5: Write Act 4 - C# Interop & Wrap-up (Minutes 30-40)
**Scenes:**
- 4.1: C# consuming F# types basics (30-33 min)
- 4.2: Pattern matching examples (33-36 min)
- 4.3: Key takeaways summary (36-38 min)
- 4.4: Resources and next steps (38-40 min)

**Dialogue focus:** Shift to "what this means for your team" angle. Both devs share closing thoughts. Include bridge dialogue option for continuing to add-ons.

**Two ending options:**
- (a) Standalone conclusion if stopping here
- (b) Transition bridge: "And we've just scratched the surface..." if continuing

### Step 6: Add Staging and Technical Cues Throughout
Review complete script adding:
- Physical position notes (`[DEV A AT PODIUM]`, `[DEV 1 NEAR SCREEN]`)
- Keyboard handoff cues (`[DEV A TAKES KEYBOARD]`)
- Audience interaction points (`[PAUSE FOR QUESTIONS]`, `[GAUGE REACTION]`)
- Window focus indicators (`[FOCUS: LEFT F# WINDOW]`, `[FOCUS: RIGHT C# WINDOW]`)

### Step 7: Review and Polish Dialogue
Read through for:
- Natural conversation flow (not lecture-y)
- Balanced speaking time between devs
- "Yes, and..." moments feel organic, not forced
- Technical accuracy of all code blocks
- Timing feasibility within 40-minute target

## Considerations

### Dialogue Voice
- **Option A:** Distinct personalities - one more skeptical/questioning, one more enthusiastic/proposing
- **Option B:** Interchangeable partners - both equally curious and excited
- **Recommendation:** Lean toward Option A for more dynamic conversation, but keep it subtle

### Code Typing vs Pre-written
- Iterations 0→1→2: Pre-written files, switched between (avoids typing errors)
- Test case outputs: Live `dotnet watch run` showing results
- Script should indicate: `[SWITCH TO Iteration1.fs]` vs `[TYPE: | VIP of RegisteredCustomer]`

### Physical Staging
- Dev A: Podium position (has notes, controls flow)
- Dev 1: Near screen (can point at code, gesture at output)
- Dev 1 may move closer to podium during discussion scenes
- Single laptop at podium, both can access

## Success Criteria
- [ ] Script can be performed in 35-40 minutes
- [ ] Dialogue feels like genuine pair programming conversation
- [ ] All "Yes, and..." transitions feel natural
- [ ] Code examples are accurate and runnable
- [ ] Has satisfying standalone conclusion
- [ ] Optional bridge to add-ons doesn't feel forced

## Dependencies
- `presentation-notes.md` (source content)
- Working demo code in `src/` folder

## Future Work (Separate Plans)
- Add-on modules (VIP, SAFE Stack, Copilot)
- Time budget guide
- Alternate "Window Owners" script variation
