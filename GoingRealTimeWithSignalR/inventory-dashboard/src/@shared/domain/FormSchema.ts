import { BaseInputProps } from '@shared/domain/BaseInputProps';

export interface FormSchemaItem<FormFields = any> extends BaseInputProps {
  fieldName: keyof FormFields;
}

export type FormSchema<FormFields> = {
  [key in keyof FormFields]: FormSchemaItem<FormFields>;
};
