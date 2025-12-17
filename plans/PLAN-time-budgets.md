# Plan: Time Budget & Configuration Guide

## Overview
Create a reference document showing how to configure the presentation for different time slots and audiences.

## Prerequisites
- Core script completed
- Add-on modules completed

## Output
`presentation-time-budgets.md` - Quick reference for presenters

---

## Configurations

### Configuration A: Core Only (35-40 min)
**Use case:** Short slot, lunch-and-learn, intro session

**Content:**
- Core script Acts 1-4
- Standalone wrap-up ending

**Modules:** None

---

### Configuration B: Core + VIP (45 min)
**Use case:** Standard meetup slot

**Content:**
- Core script Acts 1-3
- VIP Refactoring add-on
- Core Act 4 (C# Interop + Wrap-up)

**Audience:** General .NET developers

---

### Configuration C: Core + C# Deep-dive (50 min)
**Use case:** C#-focused user group

**Content:**
- Core script Acts 1-4
- VIP Refactoring add-on
- Advanced C# Patterns add-on

**Audience:** Teams evaluating F# adoption from C# codebase

---

### Configuration D: Core + SAFE Intro (50 min)
**Use case:** Web development audience

**Content:**
- Core script Acts 1-4
- SAFE Intro add-on
- SAFE Elmish add-on (abbreviated)

**Audience:** Full-stack developers, web-focused

---

### Configuration E: Full Journey (60 min)
**Use case:** Full conference slot

**Content:**
- Core script Acts 1-4
- VIP Refactoring add-on
- SAFE Intro add-on
- SAFE Elmish add-on

**Audience:** General, want complete picture

---

### Configuration F: Extended (75+ min)
**Use case:** Workshop, extended session

**Content:**
- Everything from Configuration E
- SAFE Server add-on
- Copilot add-on (if brave)
- Extended Q&A

**Audience:** Highly engaged, want hands-on depth

---

## Add-on Compatibility Matrix

| Add-on | Can follow Core | Can follow VIP | Can follow C# Patterns | Can follow SAFE Intro | Can follow SAFE Elmish |
|--------|-----------------|----------------|------------------------|----------------------|------------------------|
| VIP Refactoring | ✅ (after Act 3) | - | ❌ | ❌ | ❌ |
| Advanced C# | ✅ (in Act 4) | ✅ | - | ❌ | ❌ |
| SAFE Intro | ✅ (after Act 4) | ✅ | ✅ | - | ❌ |
| SAFE Server | ❌ | ❌ | ❌ | ✅ | ❌ |
| SAFE Elmish | ❌ | ❌ | ❌ | ✅ | - |
| Copilot | ❌ | ❌ | ❌ | ❌ | ✅ |

---

## Audience Targeting Notes

### C#-heavy audience
- Emphasize: Core + VIP + Advanced C# Patterns
- De-emphasize: SAFE Stack (mention as "explore later")

### F#-curious beginners
- Emphasize: Core only, slower pace, more explanation
- De-emphasize: VIP (might feel like too much)

### Web developers
- Emphasize: Core + SAFE modules
- De-emphasize: Advanced C# Patterns

### Architecture-focused
- Emphasize: Core + SAFE Intro (shared types, no DTOs)
- De-emphasize: Copilot (too tactical)

---

## Time Check Cues
Script should include time check points:
- [ ] Minute 15: Should be starting Iteration 1
- [ ] Minute 25: Should be finishing Iteration 2
- [ ] Minute 35: Should be in C# Interop section
- [ ] Minute 40: Decision point - wrap up or continue to add-ons

If behind schedule:
- Abbreviate benefits discussion
- Skip wrapper patterns in C# Interop
- Use pre-built code instead of switching files

## Success Criteria
- [ ] Clear guidance for each time slot
- [ ] Presenters can quickly pick configuration
- [ ] Compatibility issues are obvious
- [ ] Audience targeting is actionable
