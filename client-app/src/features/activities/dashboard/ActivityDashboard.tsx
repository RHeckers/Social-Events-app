import React, { SyntheticEvent } from "react";
import { Grid } from "semantic-ui-react";
import { IActivity } from "../../../app/interfaces/IActivity";
import ActivityList from './ActivityList';
import ActivityDetails from './../details/ActivityDetails';
import ActivityForm from './../form/ActivityForm';

interface IPros {
    activities: IActivity[];
    selectedActivity: IActivity | null;
    editMode: boolean;
    selectActivity: (id: string) => void;
    setEditMode: (editMode: boolean) => void;
    setActivities: (activity: IActivity | null) => void;
    editActivity: (activity: IActivity) => void;
    createActivity: (activity: IActivity) => void;
    deleteActivity: (e: SyntheticEvent<HTMLButtonElement>, id: string) => void;
    submitting: boolean;
    target: string
}

const ActivityDashboard: React.FC<IPros> = ({
    activities, 
    selectActivity, 
    selectedActivity,
    editMode,
    setEditMode,
    setActivities,
    editActivity,
    createActivity,
    deleteActivity,
    submitting,
    target
  }) => {
  return (
    <Grid>
      <Grid.Column width={10}>
          <ActivityList 
            activities={activities} 
            submitting={submitting}
            target={target}
            deleteActivity={deleteActivity}
            selectActivity={selectActivity} />
      </Grid.Column>
      <Grid.Column width={6}>
         {selectedActivity && !editMode && (
            <ActivityDetails 
              setActivities={setActivities} 
              activity={selectedActivity} 
              setEditMode={setEditMode} />
          )}
         {editMode && 
            <ActivityForm 
              submitting={submitting}
              key={selectedActivity && selectedActivity.id || 0}
              setEditMode={setEditMode} 
              activity={selectedActivity!} 
              createActivity={createActivity}
              editActivity={editActivity}
              />}
      </Grid.Column>
    </Grid>
  );
};

export default ActivityDashboard;
