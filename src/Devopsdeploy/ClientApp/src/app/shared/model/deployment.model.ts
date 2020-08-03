import { Moment } from 'moment';

export interface IDeployment {
  id?: string;
  deployedAt?: string;
  releaseId?: string;
  environmentId?: string;
}

export const defaultValue: Readonly<IDeployment> = {};
