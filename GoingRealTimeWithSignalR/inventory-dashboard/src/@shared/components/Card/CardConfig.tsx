import React from 'react';

export interface CardConfig {
  name: string,
  label?: string,
  content: (item: any) => React.ReactNode,
}