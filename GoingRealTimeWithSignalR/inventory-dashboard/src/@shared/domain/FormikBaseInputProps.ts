import { BaseInputProps } from './BaseInputProps';
import { FormSchemaItem } from './FormSchema';

export type FormikBaseInputProps = BaseInputProps & {
  schema: FormSchemaItem;
};
