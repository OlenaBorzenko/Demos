import { Subject } from 'rxjs';

export interface BaseInputProps {
  label?: string;
  isRequired?: boolean;
  restrictEngine?: (value: any) => any;
  placeholder?: string;
  maxLength?: number;
  shouldBeScrolledIntoView?: Subject<any>;
  disabled?: boolean;
  className?: string;
  isNumeric?: boolean;
  autoFocus?: boolean;
  readOnly?: boolean;
  format?: (value: any) => any;
}
