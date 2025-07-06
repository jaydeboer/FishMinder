# FishMinder MVP Implementation - Agent Instructions

## Overview
This document provides detailed instructions for implementing the FishMinder MVP. The project uses a structured task management system to track progress and ensure all requirements are met.

## Task Management System

### Task States
- `[ ]` = NOT_STARTED - Tasks not yet begun
- `[/]` = IN_PROGRESS - Tasks currently being worked on  
- `[x]` = COMPLETE - Tasks finished and verified
- `[-]` = CANCELLED - Tasks no longer relevant

### Working with Tasks

#### Viewing Current Tasks
Use `view_tasklist` to see the current task hierarchy and progress.

#### Updating Task Progress
When starting work on a new task:
```
update_tasks({
  "tasks": [
    {"task_id": "previous-task-id", "state": "COMPLETE"},
    {"task_id": "current-task-id", "state": "IN_PROGRESS"}
  ]
})
```

#### Marking Requirements Complete
When completing features, update both the task system AND the REQUIREMENTS.md file:
1. Mark the task as COMPLETE in the task system
2. Update the corresponding checkboxes in REQUIREMENTS.md from `- [ ]` to `- [x]`
3. Update the MVP.md progress tracking

## Implementation Workflow

### Phase 1: Project Infrastructure Setup
**Current Status:** IN_PROGRESS

#### Step 1.1: Initialize Blazor WebAssembly PWA Project
```bash
dotnet new blazorwasm --pwa -n FishMinder
cd FishMinder
```

**Requirements to mark complete:**
- REQUIREMENTS.md line 49: `- [x] Built using Blazor framework (Server or WebAssembly)`
- REQUIREMENTS.md line 56: `- [x] PWA manifest file configuration`

#### Step 1.2: Configure Project Dependencies
Add these NuGet packages:
```bash
dotnet add package Blazored.LocalStorage
dotnet add package Microsoft.AspNetCore.Components.WebAssembly.DevServer
dotnet add package System.Net.Http.Json
```

#### Step 1.3: Set Up Project Structure
Create folders:
- `/Models` - Data entities
- `/Services` - Business logic and data access
- `/Components` - Reusable UI components
- `/Pages` - Main application pages
- `/Data` - Static data and configurations

### Phase 2: Core Data Models and Storage

#### Step 2.1: Create Core Data Models
Implement these entities in `/Models`:

**Angler.cs**
```csharp
public class Angler
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;
}
```

**FishCatch.cs**
```csharp
public class FishCatch
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string AnglerId { get; set; }
    public string SpeciesId { get; set; }
    public DateTime CaughtAt { get; set; } = DateTime.UtcNow;
    public bool IsKept { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string TripId { get; set; }
}
```

**Requirements to mark complete:**
- REQUIREMENTS.md line 82: `- [x] Fishing trip session entity`
- REQUIREMENTS.md line 83: `- [x] Angler profile data structure`
- REQUIREMENTS.md line 84: `- [x] Fish catch record schema`

#### Step 2.2: Implement IndexedDB Data Access Layer
Create `/Services/DataService.cs` with repository pattern for CRUD operations.

**Requirements to mark complete:**
- REQUIREMENTS.md line 65: `- [x] IndexedDB for complex data structures`
- REQUIREMENTS.md line 91: `- [x] Complete fishing trip data storage`

#### Step 2.3: Create Species Database
Populate `/Data/SpeciesData.cs` with Minnesota and North Dakota fish species.

**Requirements to mark complete:**
- REQUIREMENTS.md line 99: `- [x] Preloaded species database for Minnesota waters`
- REQUIREMENTS.md line 100: `- [x] Preloaded species database for North Dakota waters`

### Phase 3: Basic UI Framework and Navigation

#### Step 3.1: Build Responsive Layout Components
Create main layout with mobile-first approach in `/Shared/MainLayout.razor`.

**Requirements to mark complete:**
- REQUIREMENTS.md line 74: `- [x] Responsive design for various screen sizes`
- REQUIREMENTS.md line 119: `- [x] Simple, intuitive navigation`

#### Step 3.2: Develop Touch-Optimized UI Components
Ensure all interactive elements meet 44px minimum touch target size.

**Requirements to mark complete:**
- REQUIREMENTS.md line 73: `- [x] Large touch targets (minimum 44px)`
- REQUIREMENTS.md line 130: `- [x] Glove-friendly touch targets`

### Progress Tracking Instructions

#### Updating MVP.md Progress
After completing each phase, update the progress tracking in MVP.md:

1. Change phase status from ⏳ Pending to ✅ Complete
2. Update overall MVP progress percentage
3. Update current phase and next milestone
4. Add any notes about challenges or deviations

#### Updating REQUIREMENTS.md
For each implemented feature:
1. Find the corresponding requirement line
2. Change `- [ ]` to `- [x]`
3. Commit changes with descriptive messages

#### Example Progress Update Workflow
```
1. Start working on angler management
   - update_tasks: Mark "Angler Management System" as IN_PROGRESS
   
2. Complete angler add functionality
   - Mark REQUIREMENTS.md line 9: `- [x] Support multiple anglers on a single boat/trip`
   - Mark REQUIREMENTS.md line 10: `- [x] Add/remove anglers from current fishing session`
   
3. Complete entire angler management phase
   - update_tasks: Mark "Angler Management System" as COMPLETE
   - Update MVP.md Phase 4 status to ✅ Complete
   - Move to next phase
```

## Quality Assurance Guidelines

### Testing Requirements
- Write unit tests for all service classes
- Test offline functionality thoroughly
- Validate data persistence across browser sessions
- Test on multiple mobile devices and screen sizes

### Code Quality Standards
- Follow C# naming conventions
- Use dependency injection for services
- Implement proper error handling
- Add XML documentation for public APIs
- Maintain separation of concerns

### Performance Targets
- App launch time < 3 seconds
- Touch response time < 100ms
- Smooth 60fps animations
- Minimal memory usage
- Battery optimization

## Deployment Checklist

### Pre-Deployment Validation
- [ ] All MVP requirements marked complete in REQUIREMENTS.md
- [ ] All tasks marked complete in task management system
- [ ] MVP.md shows 100% completion
- [ ] Unit tests passing
- [ ] PWA installation working
- [ ] Offline functionality verified
- [ ] Performance targets met

### Documentation Requirements
- [ ] User quick start guide created
- [ ] Feature documentation complete
- [ ] Technical architecture documented
- [ ] Deployment guide written
- [ ] Troubleshooting guide available

## Success Metrics Validation

Before marking MVP complete, verify:
- [ ] Multi-angler support functional
- [ ] Catch entry takes < 10 seconds
- [ ] Species counting accurate
- [ ] GPS coordinates captured
- [ ] App works 100% offline
- [ ] PWA installs successfully
- [ ] Touch targets meet 44px minimum
- [ ] App launches in < 3 seconds

## Next Steps After MVP

Once MVP is complete and validated:
1. Gather user feedback
2. Plan Phase 2 enhancements
3. Consider cloud synchronization
4. Evaluate additional features from REQUIREMENTS.md
5. Plan production deployment strategy

## Emergency Procedures

If critical issues arise:
1. Mark affected tasks as IN_PROGRESS
2. Document the issue in task descriptions
3. Update MVP.md with risk assessment
4. Prioritize fixes based on MVP success criteria
5. Consider scope adjustments if necessary

Remember: The goal is a functional, reliable MVP that meets core user needs. Quality over feature completeness.
