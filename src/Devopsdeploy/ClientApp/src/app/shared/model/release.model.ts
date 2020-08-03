import { Moment } from 'moment';
import {IDeployment} from "app/shared/model/deployment.model";

export interface IRelease {
  id?: string;
  version?: string;
  created?: string;
  projectId?: string;
  deployments?: Array<IDeployment>;
}

export const defaultValue: Readonly<IRelease> = {};
