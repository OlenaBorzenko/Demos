import React from 'react';

export interface NestedCardConfig {
  name: string,
  label?: string,
  content: (item: any) => React.ReactNode,
}