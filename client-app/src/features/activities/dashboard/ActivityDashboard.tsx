import React from "react";
import { Grid, List } from "semantic-ui-react";
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
}

const ActivityDashboard: React.FC<IPros> = ({
    activities, 
    selectActivity, 
    selectedActivity,
    editMode,
    setEditMode,
    setActivities
  }) => {
  return (
    <Grid>
      <Grid.Column width={10}>
          <ActivityList activities={activities} selectActivity={selectActivity} />
      </Grid.Column>
      <Grid.Column width={6}>
         {selectedActivity && !editMode && (
            <ActivityDetails 
              setActivities={setActivities} 
              activity={selectedActivity} 
              setEditMode={setEditMode} />
          )}
         {editMode && <ActivityForm setEditMode={setEditMode} activity={selectedActivity!} />}
      </Grid.Column>
    </Grid>
  );
};

export default ActivityDashboard;
